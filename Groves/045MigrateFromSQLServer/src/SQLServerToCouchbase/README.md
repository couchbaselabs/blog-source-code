# SQL Server Setup

Make sure to create a database called "sqltocb".

Run the SQL scripts in the SQLServerDataAccess projects to create the necessary tables.

# Couchbase Setup

Make sure to create a bucket called "sqltocb", and create a primary index on it (CREATE PRIMARY INDEX on `sqltocb`).

To switch between SQL and CB, change the WhichDatabase setting in Web.config between "SQLServer" and "CouchbaseServer".
