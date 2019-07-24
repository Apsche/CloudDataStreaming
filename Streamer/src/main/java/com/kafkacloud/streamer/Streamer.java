/***
This is the Kafka Streams application
mvn install
java -cp target/Streamer-1.0-SNAPSHOT-shaded.jar com.kafkacloud.streamer.Streamer
***/
package com.kafkacloud.streamer;


import org.apache.kafka.common.serialization.Serdes;
import org.apache.kafka.common.config.SaslConfigs;
import org.apache.kafka.streams.KafkaStreams;
import org.apache.kafka.streams.kstream.KStream;
import org.apache.kafka.streams.kstream.Materialized;
import org.apache.kafka.streams.kstream.Produced;
import org.apache.kafka.streams.StreamsBuilder;
import org.apache.kafka.streams.StreamsConfig;
import org.apache.kafka.streams.Topology;
import org.apache.kafka.clients.producer.ProducerConfig;
import org.apache.kafka.streams.kstream.JoinWindows;
import org.apache.kafka.streams.kstream.Joined;
import org.apache.kafka.streams.KafkaStreams;
import org.apache.kafka.streams.kstream.Printed;
import org.apache.avro.Schema;
import org.apache.avro.generic.GenericData;
import org.apache.avro.generic.GenericRecord;

import io.confluent.kafka.serializers.KafkaAvroSerializer;

import java.util.concurrent.CountDownLatch;
import java.util.Properties;
import java.time.Duration;
import org.json.*;
import java.sql.Timestamp;
import java.text.SimpleDateFormat;
import java.util.Date;

public class Streamer
{
	public static void main(String args[]) throws Exception
	{
		// Declare the properties of the streams processor
		// The configuration values defined in StreamsConfig must be mapped
		Properties props = new Properties();
		props.put(StreamsConfig.APPLICATION_ID_CONFIG, "streamer");
		props.put(StreamsConfig.BOOTSTRAP_SERVERS_CONFIG, "pkc-ldrz4.us-east-2.aws.confluent.cloud:9092");
		props.put(StreamsConfig.DEFAULT_KEY_SERDE_CLASS_CONFIG, Serdes.String().getClass().getName());
		props.put(StreamsConfig.DEFAULT_VALUE_SERDE_CLASS_CONFIG, Serdes.String().getClass().getName());
		props.put(StreamsConfig.SECURITY_PROTOCOL_CONFIG, "SASL_SSL");
		props.put(StreamsConfig.REPLICATION_FACTOR_CONFIG, "3");
		props.put(SaslConfigs.SASL_MECHANISM, "PLAIN");
		props.put(SaslConfigs.SASL_JAAS_CONFIG, "org.apache.kafka.common.security.plain.PlainLoginModule required username=\"7BYZ6FIYJIJQ7NCR\" " +
			"password= \"Du+0pUmUl5rabqiJQGO20EBlpTHhk7AnEpC2V02WuljKhw0hQBdnE7uKUzGK5zkY\";");
		props.put("acks", "1");
		//props.put(StreamsConfig.producerPrefix(ProducerConfig.REQUEST_TIMEOUT_MS_CONFIG), 20000); //this is causing an error

		// Defines computational logic of streams builder
		final StreamsBuilder builder = new StreamsBuilder();

		// Get the stream from the first topic
		KStream<String, String> trafficSource = builder.stream("Traffic");

		// Get the stream from the second topic
		KStream<String, String> weatherSource = builder.stream("Weather");

		// Join the two streams on the location
		KStream<String, String> joined = trafficSource.join(weatherSource, (leftValue, rightValue)
			-> trimJSON(leftValue, rightValue),
			JoinWindows.of(Duration.ofMinutes(2)),
			Joined.with(
				Serdes.String(), /*key*/
				Serdes.String(), /*Left (traffic) value*/
				Serdes.String()) /*Right (weather) value*/
			);

		// Set the output topic
		// The data from the joined stream will be sent here
		joined.to("Output");

		// Get the created topology from the builder
		final Topology top = builder.build();

		// Print the topology
		System.out.println(top.describe());

		// Construct streams client with the above constructed properties
		final KafkaStreams streams = new KafkaStreams(top, props);

		// Create a shutdown hook to capture a user interrupt
		final CountDownLatch latch = new CountDownLatch(1);

		// Attatch shutdown handler to catch control
		Runtime.getRuntime().addShutdownHook(new Thread("streams-shutdown-hook")
		{
			@Override
			public void run()
			{
				streams.close();
				latch.countDown();
			}
		});

		try
		{
			streams.cleanUp();
			streams.start();
			latch.await();
		}
		catch (Throwable e)
		{
			System.out.println(e);
			System.exit(1);
		}
		System.exit(0);
	}

	public static String trimJSON(String traffic, String weather)
	{
		// Parse the JSON strings to JSON
		JSONObject trafficJSON = new JSONObject(traffic);
		JSONObject weatherJSON = new JSONObject(weather);

		// Create the new JSON object
		JSONObject jObject = new JSONObject();

		// Find the fields in two JSONs that we want and add them into our new JSON
		jObject.put("timestamp", new Timestamp(System.currentTimeMillis()).getTime()); // Timestamp
		jObject.put("averageDrivingSpeed", trafficJSON.getJSONObject("flowSegmentData").get("currentSpeed")); // Driver Speed
		jObject.put("driverConfidence", trafficJSON.getJSONObject("flowSegmentData").get("confidence")); // Driver Confidence
		jObject.put("weatherDescription", weatherJSON.getJSONArray("weather").getJSONObject(0).get("description")); // Weather Description
		jObject.put("temp", weatherJSON.getJSONObject("main").get("temp")); // Temperature
		jObject.put("humidity", weatherJSON.getJSONObject("main").get("humidity")); // Humidity
		jObject.put("windSpeed", weatherJSON.getJSONObject("wind").get("speed")); // Wind Speed
		jObject.put("windDeg", weatherJSON.getJSONObject("wind").get("deg")); // Wind Direction
		jObject.put("clouds", weatherJSON.getJSONObject("clouds").get("all")); // Clouds
		jObject.put("location", weatherJSON.get("name")); // Location Name

		// Turn the JSON back into a string and return
		return jObject.toString();
	}
}
