<h1 align="center">MechShops</h1>

## Descrição do Projeto
<p align="center">Projeto relacionado ao teste técnico do Sparta Labs. Solução backend com um conjunto de APIs para controlar uma rede de oficinas, feito com .NET 6.</p>

## Sumário
=================
<!--ts-->
   * [Features](#features)
   * [Get started](#instalacao)
   * [Orientações](#orientaçoes)
   * [Tests](#testes)
<!--te-->

<h2 id="features">Features</h2>
<p>- ✅ Requisitos obrigatórios</p>
<p>- ✅ Oficina pode ter serviços personalizados ✨</p>

<h2 id="instalacao">Get started</h2>

Antes de começar, você vai precisar ter instalado em sua máquina as seguintes ferramentas:
[.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0), [Docker](https://docs.docker.com/desktop/windows/install/)(opcional). 

### 🎲 Rodando a API

```bash
# Crie um container SQL Server no Docker (opcional)
$ docker pull mcr.microsoft.com/mssql/server:2019-latest
$ docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<PASSWORD>" -p 1433:1433 --name <NOME> -h <HOST> -d mcr.microsoft.com/mssql/server:2019-latest

$ Configure sua connection string no arquivo appsettingsDevelopment.json

# Clone este repositório
$ git clone https://github.com/lyrapedro/mechshops.git

# Acesse a pasta da solução no terminal/cmd
$ cd mechshops
$ cd src

# Instale as dependências
$ dotnet restore

# Aplique as migrations ao seu banco de dados
$ dotnet ef database update

# Rode a aplicação
$ dotnet run

# O servidor iniciará na porta:7213 - faça as requisições para: <http://localhost:7213>
```

<h2 id="orientaçoes">Orientações</h2>

A collection do Postman com todas as requisições está em um arquivo JSON na pasta raiz da solução.


