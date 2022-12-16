SET PGPASSWORD=123

cd /D C:\Program Files

cd PostgreSQL

cd 13

cd bin

pg_restore.exe --host localhost --port 5432 --username postgres --dbname "ClothesShopRestore" --verbose "D:\567.backup"
