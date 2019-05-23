# AWS Step Functions Test

Teste com Step Functions do AWS.

veja: [Building Lambda Functions with C#](https://docs.aws.amazon.com/lambda/latest/dg/dotnet-programming-model.html)

## 1. Configuração do ambiente

Pré requisitos:
- [python / pip / aws cli](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-install.html)
- [dotnet core](https://dotnet.microsoft.com/download)

### Configuração do AWS CLI

Crie um usuário específico com o nível de acesso adequado, utilizando o Console AWS. Configure o AWS CLI para este usuário e region ajustado para São Paulo:

```code
$ aws configure
AWS Access Key ID [None]: key_criada_na_tela_de_administracao_de_usuarios
AWS Secret Access Key [None]: secret_criada_na_tela_de_administracao_de_usuarios
Default region name [None]: sa-east-1
Default output format [None]: json
```

Efetuada esta configuração, as chamadas para o AWS utilizando o aws cli irão usar este key/secret/region/format para acessar os recursos.

### Configurar o dotnet para poder criar projetos para o AWS Lambda

Para verificar se o dotnet cli já tem os templates da AWS execute o seguinte comando:

```console
dotnet new -all
```

Deve listar todos os templates disponíveis, dentre os quais podem estar os da AWS, iniciando como ```Lambda Simple DynamoDB```, ```Lambda ASP.NET Core Web API```, ```Serverless Simple S3 Function```, ```Step Function Hello World```, dentre outros. Caso não esteja listando os adicionais da AWS, basta instalar via o pacote nuget Amazon.Lambda.Templates. Para instalar estes templates, execute o seguinte comando:

```console
dotnet new -i Amazon.Lambda.Templates
```

### Adicionar o ferramental para fazer deploy para o AWS

Para fazer o deploy para o AWS é preciso instalar o .NET Global Tool com o seguinte comando:

```console
dotnet tool install -g Amazon.Lambda.Tools
```

Caso já esteja instalado, verifique se está com a última versão:

```console
dotnet tool update -g Amazon.Lambda.Tools
```


## 2. Criação e execução de uma Lambda Function

### Criar uma Role para executar Lambda

Para poder executar uma Lambda é preciso ter uma Role configurada.
Veja: [AWS Lambda Execution Role](https://docs.aws.amazon.com/lambda/latest/dg/lambda-intro-execution-role.html_)

### Criar um novo projeto Lambda Function vazio

Você pode usar o projeto MyFunction, ou criar seu próprio projeto.
Para criar um novo projeto, informando ao dotnet cli para usar o template lambda.EmptyFunction, com o nome MyFunction, com o profile apontando para o Role criado anteriormente e com a region para São Paulo:

```console
dotnet new lambda.EmptyFunction --name MyFunction --profile lambda-role --region sa-east-1
```

### Deploy para o AWS

Faça o deploy do lambda function criado para o AWS, informado ao dotnet cli para usar o MyFunction, usar o role lambda-role (criado anteriormente) e a region São Paulo:

```console
dotnet lambda deploy-function MyFunction –-function-role lambda-role --region sa-east-1
```

### Teste de execução do lambda

Para testar o lambda que acabou de ser feito o deploy, execute o seguinte comando:

```console
dotnet lambda invoke-function MyFunction --payload "Just Checking If Everything is OK"
```

O resultado deve ser algo +/- assim:

```console
dotnet lambda invoke-function MyFunction --payload "Just Checking If Everything is OK"
Payload:
"JUST CHECKING IF EVERYTHING IS OK"

Log Tail:
START RequestId: id Version: $LATEST
END RequestId: id
REPORT RequestId: id  Duration: 0.99 ms       Billed Duration: 100 ms         Memory Size: 256 MB     Max Memory Used: 12 MB
```

## 3. Criação e execução um Step Function

### Criando um projeto para Step Functions

Você pode usar o projeto MyStepFunction ou criar seu próprio projeto.
A criação e execução com sucesso o projeto simples de Lambda Function garante que a infra-estrutura necessária está configurada e ok. Crie um projeto "vazio" utilizando o template do Step Functions:

```console
dotnet new serverless.StepFunctionsHelloWorld --name MyStepFunction --profile lambda-role --region sa-east-1
```

Isso irá criar um projeto contendo:

- `serverless.template` - Um template para o AWS CloudFormation declarando a function Serverless e outros recursos necessários. Neste arquivo que devem ser declarados os Lambda Functions que a State Machine do Step Function irá chamar. É importante que cada CludFormation seja único por projeto, um para o Step Functions, outro para WebAPI, etc.
- `state-machine.json` - A definição do workflow (state machine) desta Step Function.
- `StepFunctionTasks.cs` - Essa classe contém as Lambda Functions que a state machine do Step Function irá chamar.
- `State.cs` - Essa classe representa o estado das execuções do Step Function entre cada cgamada das Lambda Functions. É nela que devem ser criadas as propriedades correspondentes ao JSON passado como parâmetro no momento da execução.
- `aws-lambda-tools-defaults.json` - argumentos padrão para deploy via Visual Studio ou CLI.

Um projeto de teste também deve ter sido criado.
O projeto gerado é um exemplo simples Hello World do Step Functions. 
São geradas 2 funções Lambda que são chamadas como Tasks na state machine. 
No arquivo `state-machine.json` é definida a State Machine que informa ao Step Function em que ordem deve ser chamada as Lambda Functions. 
O estado de execução da Step Function é mantido no objeto `State` que é de onde a Lambda Function lê, grava e retorna os dados.
No exemplo a primeira função Lambda também retorna o tempo de espera para mostrar como configurar uma pausa na State Machine.

### Deploy do projeto Step Functions

Antes de realizar o deploy é recomendável ter criado um Bucket S3 para armazenar os assemblies, caso contrário o processo de deploy irá criar um automaticamente com acesso público. Para realizar o deploy execute o seguinte comando (troque o valore de --s3-bucket):

```console
cd [caminho].../MyStepFunction/src/MyStepFunction
dotnet lambda deploy-serverless --s3-bucket stepfunctions-tests --stack-name MeuCloudFormationStackName
```
Os parâmetros do comando acima podem ser especificados no arquivo `aws-lambda-tools-defaults.json" e assim não precisam ser informados na linha de comando.
O Step Function e demais recursos serão criados conforme a necessidade, e estarão disponíveis para testes no Console AWS, na região especificada para criação.

Obs.: Atualizações ao Step Function podem ser feitas executando o mesmo comando do deploy.

## 4. Iniciar um Step Function programaticamente

Você pode usar o projeto MyStepFunctionStarter para iniciar o Step Function, passar parâmetros e recuperar o resultado final do processamento.

## 5. Criação de uma WebAPI 

### Criando um projeto WebAPI

Você pode usar o projeto RandomGenerator ou criar seu próprio projeto.

```console
dotnet new serverless.AspNetCoreWebAPI -n RandomGenerator
```

Isso irá criar um projeto contendo:

- `serverless.template` - um template para o AWS CloudFormation declarando as funções e outros recursos AWS. É importante que cada CloudFormation seja único por projeto, um para Step Functions, outro para WebAPI, etc.
- `aws-lambda-tools-defaults.json` - configurações padrão para usar com o Visual Studio e CLI.
- `LambdaEntryPoint.cs` - classe que deriva de **Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction**. Contém o código pra fazer a preparação (bootstrap) do ASP.NET Core hosting framework. A função Lambda é definida na classe base. Altere a classe base para **Amazon.Lambda.AspNetCoreServer.ApplicationLoadBalancerFunction** quando estiver usando um Application Load Balancer.
- `LocalEntryPoint.cs` - Usado para desenvolvimento local, com funções para bootstrap do ASP.NET Core hosting framework com Kestrel.
- `Startup.cs` - Classe ASP.NET Core Startup usada para configurar os serviços que o ASP.NET Core vai utilizar.
- `web.config` - Usado para desenvolvimento local.
- `Controllers\S3ProxyController` - Controller pra proxying com o bucket S3.
- `Controllers\ValuesController` - Exemplo de um controller Web API.

### Deploy do projeto WebAPI

Antes de realizar o deploy é recomendável ter criado um Bucket S3 para armazenar os assemblies, caso contrário o processo de deploy irá criar um automaticamente com acesso público. Para realizar o deploy execute o seguinte comando (troque o valore de --s3-bucket):

Edite o arquivo aws-lambda-tools-defaults.json, e configure os valores para os parâmetros:

``code
"region": "sa-east-1",
"stack-name": "WebAPITestsCloudFormationStack"
"s3-bucket": "webapi-tests"
```
Para publicar basta executar o comando a seguir:

```console
cd RandomGenerator/src/RandomGenerator
dotnet lambda deploy-serverless 
```

No final do processo de deploy a URL para acesso é exibida.
A webapi e demais recursos são criados conforme a necessiade e estarão disponíveis para testes no Console AWS, na região especificada para criação.

