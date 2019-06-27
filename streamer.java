/***
This is the Kafka Streams application
ToDo:
  Change <key,value> data types to appropriate data type
***/

import org.apache.kafka.common.serialization.Serdes;
import org.apache.kafka.streams.KafkaStreams;
import org.apache.kafka.streams.StreamsBuilder;
import org.apache.kafka.streams.StreamsConfig;
import org.apache.kafka.streams.Topology;

import java.util.Properties;
import java.time.Duration;

public Class streamer
{
	public static void main(String args[]) throws Exception
	{
		// Declare the properties of the streams processor
		// The configuration values defined in StreamsConfig must be mapped
		Properties props = new Properties();
		props.put(StreamsConfig.APPLICATION_ID_CONFIG, "streamer");
		props.put(StreamsConfig.BOOTSTRAP_SERVERS_CONFIG, "localhost:9092");
		props.put(StreamsConfig.KEY_SERDE_CLASS_CONFIG, Serdes.String().getClass().getName());
		props.put(StreamsConfig.VALUE_SERDE_CLASS_CONFIG, Serdes.String().getClass().getName());

		// Defines computational logic of streams builder
		final StreamsBuilder builder = new StreamsBuilder();

		// Get the stream from the first topic
		KStream<String, String> trafficSource = builder.stream("streams-traffic-input");

		// Get the stream from the second topic
		KStream<String, String> weatherSource = builder.stream("streams-weather-input");

		// Join the two streams on the location
		KStream<String, String> joined = trafficSource.join(weatherSource, (leftValue, rightValue)
			-> "traffic=" + leftValue + ", weather=" + rightValue,
			JoinWindows.of(Duration.ofMinutes(5)),
			Joined.with(
				Serdes.String(), /*key*/
				Serdes.String(), /*Left (traffic) value*/
				Serdes.String()) /*Right (weather) value*/
			);

		// Set the output topic
		// The data from the joined stream will be sent here
		joined.to("streams-output");

		// Get the created topology from the builder
		final Topology top = builder.build();

		// Print the topology
		System.out.println(top.describe());

		// Construct streams client with the above constructed properties
		final KafkaStreams streams = KafkaStreams(top, props);

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
			streams.start();
			latch.await();
		}
		catch (Throwable e)
		{
			System.exit(1);
		}
		System.exit(0);
	}
}
