#!/bin/bash
set -e

host="$1"
shift
cmd="$@"

echo "Waiting for SQL Server at $host..."

until /opt/mssql-tools/bin/sqlcmd -S "$host" -U sa -P "YourPassword123!" -Q "SELECT 1" > /dev/null 2>&1; do
  >&2 echo "SQL Server is unavailable - sleeping"
  sleep 5
done

>&2 echo "SQL Server is up - executing command"
exec $cmd
