# aws_step_functions_test
Teste com Step Functions do AWS

veja: [Building Lambda Functions with C#](https://docs.aws.amazon.com/lambda/latest/dg/dotnet-programming-model.html)

pre-reqs:
    - python
    - pip
    - aws cli

Precisa configurar o AWS CLI para um usuário com poderes adequados e region ajustado para São Paulo:

```code
$ aws configure
AWS Access Key ID [None]: key_criada_na_tela_de_administracao_de_usuarios
AWS Secret Access Key [None]: secret_criada_na_tela_de_administracao_de_usuarios
Default region name [None]: sa-east-1
Default output format [None]: json
```

Precisa criar um usuário com os acesso corretos ao AWS para conseguir subir os Lambda Functions.

## Criar uma Role para executar Lambda

veja: [AWS Lambda Execution Role](https://docs.aws.amazon.com/lambda/latest/dg/lambda-intro-execution-role.html_)

## Configurar o dotnet para poder criar projetos para o AWS Lambda

Para verificar se o dotnet cli já tem os templates da AWS execute o seguinte comando:

```console
dotnet new -all
```

Deve listar todos os templates disponíveis, dentre os quais podem estar os da AWS, iniciando como ```Lambda Simple DynamoDB```, ```Lambda ASP.NET Core Web API```, ```Serverless Simple S3 Function```, ```Step Function Hello World```, dentre outros. Caso não esteja listando os adicionais da AWS, basta instalar via o pacote nuget Amazon.Lambda.Templates. Para instalar estes templates, execute o seguinte comando:

```console
dotnet new -i Amazon.Lambda.Templates
```

## Criar um novo projeto Lambda Function vazio

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

## Criando um projeto para Step Functions

A criação e execução com sucesso o projeto simples de Lambda Function garante que a infra-estrutura necessária está configurada e ok. Podemos mover para o Step Functions, que é um pouquinho mais complicado. Iniciamos um projeto vazio:

```console
dotnet new serverless.StepFunctionsHelloWorld --name MyStepFunction --profile lambda-role --region sa-east-1
```

Isso irá criar um projeto contendo:

* serverless.template - Um template para o AWS CloudFormation declarando a function Serverless e outros recursos necessários.
* state-machine.json - A definição do workflow (state machine) desta Step Function.
* StepFunctionTasks.cs - Essa classe contém as Lambda Functions que a state machine do Step Function orá chamar.
* State.cs - Essa classe representa o estado das execuções do Step Function entre cada cgamada das Lambda Functions. É nela que devem ser criadas as propriedades correspondentes ao JSON passado como parâmetro no momento da execução.
* aws-lambda-tools-defaults.json - argumentos padrão para usar com o Visual Studio e ferramentas de CLI do AWS.

Um projeto de teste também deve ter sido criado.

O projeto gerado é um exemplo simples Hello World do Step Functions. São geradas 2 funções Lambda que são chamadas como Tasks na state machine. No arquivo state-machine.json é definida a State Machine que informa ao Step Function em que ordem deve ser chamada as Lambda Functions. O estado de execução da Step Function é mantido no objeto State que é de onde a Lambda Function lê, grava e retorna os dados. No exemplo a primeira função Lambda também retorna o tempo de espera para mostrar como configurar uma pausa na State Machine.

## Deploy do projeto Step Functions

Antes de realizar o deploy é preciso ter criado um Bucket S3 para armazenar os assemblies. Para realizar o deploy execute o seguinte comando (troque os valores de --s3-bucket e :

``` code
cd [caminho].../MyStepFunction/src/MyStepFunction --s3-bucket stepfunctions-tests 
dotnet lambda deploy-serverless
```

O Step Function e demais recursos serão criados conforme a necessidade, e estarão disponíveis do Console, na região especificada para criação ("region", em aws-lambda-tools-defaults.json).

Atualizações ao Step Function podem ser feitas executando o mesmo comando do deploy:

``` code
cd [caminho].../MyStepFunction/src/MyStepFunction
dotnet lambda deploy-serverless
```
