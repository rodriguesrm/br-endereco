# Brasil/Endereço

![Build](https://github.com/rodriguesrm/openbr-endereco/workflows/.RSoft%20CI/badge.svg?branch=master)

API de Endereços Brasileiros com base inicial atualizada de 2019.

## Stack Tecnologico

* DotNet Core 3.1 - https://docs.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-3.1!
* MongoDb - https://www.mongodb.com/

## Preparaçao do Banco [ `MongoDb` ]

### A partir do zero

Conecte-se a instância MongoDb destinada a aplicação.

1. Crie o banco de dados
    `db.createDatabase("<dbName>")`
2. Crie a collection principal de cep
    `db.createCollection("cep")`
3. Crie os índices:
    ```sh
    db.cep.createIndex({"_id" : 1}, {"name" : "_id_"});
    db.cep.createIndex({"cep" : 1}, {"name" : "cep","unique" : true});
    db.cep.createIndex({"codigoIbge" : 1}, {"name" : "codigoIbge"});
    ```
4. Importe o arquivo `ceps.json`, que pode ser extraído do arquivo `ceps.zip` disponível na pasta `dump` usando o `mongoimport`:
    `mongoimport --host=<host> --username=<user> --password=<password> --authenticationDatabase=<authDb> --db=<dbName> --collection=cep --file=<path>/ceps.json --jsonArray`
5. asdfas

### Deixando aplicação criar a base

A aplicação irá criar as collections na base MongoDb.

1. Suba a aplicação no ambiente.
2. Importe o arquivo `ceps.json`, que pode ser extraído do arquivo `ceps.zip` disponível na pasta `dump` usando o `mongoimport`:
    `mongoimport --host=<host> --username=<user> --password=<password> --authenticationDatabase=<authDb> --db=<dbName> --collection=cep --file=<path>/ceps.json --jsonArray`

### BackLog

  [x] Health-Check 
  [X] Swagger
  [X] Dump inicial base de cep
  [ ] Serilog
  [ ] Docker-Compose
  [ ] Serviço (Worker) para busca nos correios de CEPs não encontrados