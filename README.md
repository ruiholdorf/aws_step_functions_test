# aws_step_functions_test
Teste com Step Functions do AWS

veja: [Building Lambda Functions with C#](https://docs.aws.amazon.com/lambda/latest/dg/dotnet-programming-model.html)

pre-reqs:
- Python
- pip
- aws cli

Precisa configurar o AWS CLI para um usu�rio com poderes adequados e region ajustado para São Paulo:
```code
$ aws configure
AWS Access Key ID [None]: Key_criada_na_tela_de_administracao_de_usuarios
AWS Secret Access Key [None]: Secret_criada_na_tela_de_administracao_de_usuarios
Default region name [None]: sa-east-1
Default output format [None]: json
```

Precisa criar um usuário com os acesso corretos ao AWS para conseguir subir os Actions.

## Criar uma Role para executar Lambda

veja: [AWS Lambda Execution Role](https://docs.aws.amazon.com/lambda/latest/dg/lambda-intro-execution-role.html_)


## Configurar o dotnet para poder criar projetos para o AWS Lambda

O AWS Lambda Functions oferece templates adicionais, via o pacote nuget Amazon.Lambda.Templates. Para instalar este template, execute o seguinte comando:

```console
dotnet new -i Amazon.Lambda.Templates
```

## Criar um novo projeto Lambda vazio

Crie um novo projeto, informando ao dotnet cli para usar o template lambda.EmptyFunction, com o nome MyFunction, com o profile apontando para o Role criado anteriormente e com a region para São Paulo:

```console
dotnet new lambda.EmptyFunction --name MyFunction --profile lambda-role --region sa-east-1
```

## Adicionar o ferramental para fazer deploy para o AWS

Para fazer o deploy para o AWS é preciso instalar o .NET Global Tool com o seguinte comando:

```console
dotnet tool install -g Amazon.Lambda.Tools
```

Caso já esteja instalado, verifique se está com a última versão:

```console
dotnet tool update -g Amazon.Lambda.Tools
```

## Deploy para o AWS

Faça o deploy do lambda function criado para o AWS, informado ao dotnet cli para usar o MyFunction, usar o role lambda-role (criado anteriormente) e a region São Paulo:

```console
dotnet lambda deploy-function MyFunction –-function-role lambda-role --region sa-east-1
```

## Teste de execução do lambda

Para testar o lambda que acabou de ser feito o deploy, execute o seguinte comando:

```console
dotnet lambda invoke-function MyFunction --payload "Just Checking If Everything is OK"
```

E o resultado deve ser algo assim:

```console
dotnet lambda invoke-function MyFunction --payload "Just Checking If Everything is OK"
Payload:
"JUST CHECKING IF EVERYTHING IS OK"

Log Tail:
START RequestId: id Version: $LATEST
END RequestId: id
REPORT RequestId: id  Duration: 0.99 ms       Billed Duration: 100 ms         Memory Size: 256 MB     Max Memory Used: 12 MB
```
