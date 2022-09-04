# Deployment History
## Backend service

This repository includes:
- Backend API written in .NET 6.
- Dockerfile to host the API in a kubernetes environment
- Database creation script(DbCreate.sql) for MSSQL

The service is used to record the deployments of different apps.

The service needs a database which you can create using the provided script.
Also a connection to bitbucket is required for finding issues number.

github support is coming later

Service relies on commit messages to contain issues identifiers.

When there are at least 2 deployments available for an app you can see the released issues.
Released issues are found by searching commits between the two deployments.

## Usage Example

Create an app "MyApp".

Create Deployment with commitId "c1"

Create Deployment with commitId "c5"

Call "releases" API. A search will be performed on all commit messages between "c1" and "c5" using the specified regEx. All issues identifiers found in commits including commit "c5" but excluding "c1" will be returned as a release with a "c5" commitId.

# API

By default hosted at `localhost:5175`

## Create Application

### Request

    curl --request POST \
    --url http://localhost:5175/api/applications \
    --header 'Content-Type: application/json' \
    --data '{
        "name": "RepoName",
        "repoUrl": "bitbucket-repo-browsing-url",
        "storyRegEx": "regEx"
    }'

`name` is an application identifier for later deployment creation

`repoUrl` bitbucket repo url which you can see in the browser
like https://bitbucket.org/bulionchik/testapp/src/master/

Possibly adjustments are needed for self hosted servers

`storyRegEx` regular expression to find issue identifiers within commit messages. By default it is `[a-zA-Z]{1,10}-[0-9]+` matching, for example  AAA-1234

### Successful Response

    {
        "id": 1,
        "name": "RepoName",
        "repoUrl": "https://bitbucket.verivox.ads/projects/PRODUCTS/repos/consent-microservice/browse",
        "storyRegEx": "[a-zA-Z]{1,10}-[0-9]+",
        "deployments": null
    }

## Create Deployment

### Request
    curl --request POST \
    --url http://localhost:5175/api/deployments \
    --header 'Content-Type: application/json' \
    --data '{
        "commitId": "0d3d2313cf549605367d882a476b9ab57dbe5045",
        "timestamp": "2022-06-15T10:08:15.969",
        "applicationName": "RepoName",
        "branchName": "master"
    }'


### Successful Response

    {
		"id": 1,
		"commitId": "0d3d2313cf549605367d882a476b9ab57dbe5045",
		"appId": 1,
		"application": null,
		"timestamp": "2022-06-15T10:08:15.97",
		"branchName": "master"
	}


## Get Releases

### Request
    curl --request GET \
    --url http://localhost:5175/api/releases/{appId}

### Paged Request
    curl --request GET \
    --url http://localhost:5175/api/releases/{appId}/{page}/{pageSize}

### Successful Response

    [
        {
            "commitId": "cbd801eae647d09a1ae44692d98e8a0407860fa6",
            "stories": [
                "TEST-1",
                "TEST-2",
                "TEST-3",
            ],
            "timestamp": "2022-07-04T14:08:31.303"
        }
    ]