
# Todo App .Net and Couchbase NoSql

* dotnet --list-sdks

    + 5.0.401

* dotnet --list-runtimes

    Microsoft.AspNetCore.App 5.0.10 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]

    Microsoft.NETCore.App 5.0.10 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]

    Microsoft.WindowsDesktop.App 5.0.10 [C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App]

### Creating Project (CLI)
```bash
  dotnet new sln
```

```bash
  dotnet new webapi -o API
```
```bash
  dotnet sln add .\API\
```

1. Couchbase server community installed v7.0.2
2. Created new cluster. TodoApp 
    
    Creds -> Administrator:qweasd1 (password may change initialize from docker as password)
3. Created a bucket. -> TodoAppBucket
4. Added Coucbase.Extensions.DependencyInjection @v3.2.5 to API project.
5. Added Couchbase appsettings.json and Startup.cs and lifetime for close.
6. Added Microsoft.IdentityModel 7.0.0.
7. Added System.IdentityModel.Tokens.Jwt 6.15.0

## Features
[ApiController]
[Route("api/{controller}")]
## Controllers

- ## User
    - ### "api/user/signup"
        - **_userService.CreateUser**
        - **_authTokenService.CreateToken**
    - ### "api/user/login"
        - **_couchbaseService.Cluster.QueryAsync** for getting **Id**
        - **_couchbaseService.TodoCollection.GetAsync(queryId)**
    - ### "api/usertodos"
        - _couchbaseService.Cluster.QueryAsync<dynamic>( "SELECT * FROM `TodoAppBucket` WHERE userEmail=$1 and type=$2", options => options.Parameter(email).Parameter("todo")
        
        *query result in couchbase server is correct but in postman datas returning null.
        Despite a lot of research, I could not come to a conclusion.*

- ## Todos
    - ### "api/todos/{Id}"
        - **_couchbaseService.TodoCollection.GetAsync(Id)**
    - ### "api/todos" CreateTodoItem([FromBody] Todo todo)
        - **_couchbaseService.TodoCollection.InsertAsync<Todo>(Guid.NewGuid().ToString(), todo);**
    - ### "api/todos" Update(string _id, Todo todo)
        - **_couchbaseService.TodoCollection.InsertAsync<Todo>(Guid.NewGuid().ToString(), todo);**
    - ### Task DeleteTodoItem(string Id)
        - **_couchbaseService.TodoCollection.RemoveAsync(Id);**


## Services
- ### User Service

		Task<bool> UserExists(string username);
		Task<User> CreateUser(string username, string password, uint expiry);
		Task<User> GetUser(string username);
		Task<IEnumerable<Todo>> GetUserTodos(string search);
		Task<User> GetAndAuthenticateUser(string username, string password);
		Task UpdateUser(User user);

- ### Couchbase Service

		ICluster Cluster { get; }
		IBucket TodoAppBucket { get; }
		ICouchbaseCollection TodoCollection { get; }

- ### AuthToken Service

		string CreateToken(string username);
		bool VerifyToken(string encodedToken, string username);

# Backend
## Running Project from source files
Clone project.

```bash
  git clone https://github.com/ygtalp/TodoDotnetCouchbase
```

Go to API folder on terminal

```bash
  cd API
```

### Run Backend 

```bash
  dotnet watch run
```

For swagger go to:

https://localhost:5001/swagger

## Running Project With Docker Image

```bash
cd API
```

```bash
docker pull 19930101/api:latest
```

and get image id with
```bash
docker images
```

like: a9e2a93fb6df

then run 
```bash
docker run -p 8080:80 <YOUR_IMAGE_ID>
```


# DB

## Create Couchbase Db from local

Go to 
http://localhost:8091/

Login credentials: Administrator / password

- ## Create Cluster and Bucket (step: ***)
    - #### Cluster Name -> **TodoApp**
    - #### Bucket Name -> **TodoAppBucket**
    - then go to swagger url and test endpoints.

## Or Create Coucbase Db with docker-compose

#### Run
```bash
docker-compose up
```
Couchbase Admin UI: http://localhost:8091

Login credentials: Administrator / password

## Then Create Cluster and Bucket ***


## ScreenShots

![Swagger](https://raw.githubusercontent.com/ygtalp/TodoDotnetCouchbase/master/ss/swagger.png)
![Response](https://raw.githubusercontent.com/ygtalp/TodoDotnetCouchbase/master/ss/res.png)
![docker build](https://raw.githubusercontent.com/ygtalp/TodoDotnetCouchbase/master/ss/docker.png)
![docker run](https://raw.githubusercontent.com/ygtalp/TodoDotnetCouchbase/master/ss/2.png)







###

## Docker Hub URLS

Backend
https://hub.docker.com/repository/docker/19930101/api

DB
https://hub.docker.com/repository/docker/19930101/api_backend
