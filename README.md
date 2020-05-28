# Brasil/Endereço

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

API de Endereços Brasileiros com base inicial atualizada de 2019.

## Stack Tecnologico

* DotNet Core 3.1 - https://docs.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-3.1!
* MongoDb - https://www.mongodb.com/

## Preparaçao do Banco [ `MongoDb` ]

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

### BackLog

  - Health-Check
  - Swagger
  - Dump inicial base de cep
  - Serilog
  - Docker-Compose
  - Serviço (Worker) para busca nos correios de CEPs não encontrados