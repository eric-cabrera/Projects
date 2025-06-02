Useful commands:

General:
curl http://localhost:8083/connector-plugins
curl http://localhost:8083/connectors
curl http://localhost:8083/connectors?expand=status


To add or update connector plugins use curl. The plugin must must exist.
Post is for adding a new connector, Put is for updating a connector:

Examples
curl -s -X POST -H 'Content-Type: application/json' --data @mongodbSource.json http://localhost:8083/connectors
curl -s -X POST -H 'Content-Type: application/json' --data @sqlserverSource.json http://localhost:8083/connectors

curl -s -X PUT -H 'Content-Type: application/json' --data @sqlserverSourceUpdate.json http://localhost:8083/connectors/sql-source-connector/config
curl -s -X PUT -H 'Content-Type: application/json' --data @mongodbSource.json http://localhost:8083/connectors/mongodbsource/config
