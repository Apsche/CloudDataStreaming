In order to run the Kafka Streams program:
1. cd into the Streamer directory
2. Run mvn install (This will compile the program and install any dependencies needed)
3. Run java -cp target/Streamer-1.0-SNAPSHOT-shaded.jar com.kafkacloud.streamer.Streamer (This will run the program)
4. To end the prgram, use ctrl + c

Note: There is a bug right now that may prevent the program from runnning. 
To fix this, go into the systems temporary data then into kafka-streams/streamer and delete everything within that directory.
