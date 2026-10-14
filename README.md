# Testing an ASP.NET Core API with Bruno on Windows

A small demo that shows how to test an ASP.NET Core Web API with the [Bruno](https://www.usebruno.com/) API client.

- `src/ProductCatalog.Api`: a .NET 10 Web API with one controller, `ProductsController` (create, read, update, delete)
- `api-tests`: the Bruno collection that tests it. Every request is a plain-text `.bru` file you can commit to Git.
- `.github/workflows/api-tests.yml`: runs the collection in GitHub Actions on every push and pull request

The products are kept in memory to keep the demo focused on testing, so they reset when the API restarts. This is not a production pattern.

## What you need

- Windows 10 or 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (`winget install Microsoft.DotNet.SDK.10`)
- [Bruno](https://www.usebruno.com/downloads) (`winget install Bruno.Bruno`)
- [Node.js](https://nodejs.org/) 22, only for the Bruno CLI (`winget install OpenJS.NodeJS.LTS`)

Tested with .NET SDK 10.0.200, Bruno 4.2, Bruno CLI 4.2.0 and Node.js 22.14.0.

## Run the API

```powershell
git clone <this-repo-url>
cd bruno-aspnetcore-api-testing
dotnet run --project src\ProductCatalog.Api
```

The API listens on `http://localhost:5080`. Swagger UI is at `http://localhost:5080/swagger`.

## Test it with Bruno

1. Open Bruno and choose **Open Collection**.
2. Select the `api-tests` folder.
3. In the top-right environment list, pick **Local**.
4. Right-click the **Product API** collection and choose **Run** to run all requests.

The requests run in this order. **Create Product** must run first, because it saves the new id that the next requests use.

| Request | What it checks |
|---|---|
| Get All Products | returns 200 and a list |
| Create Product | returns 201, saves the new id as `productId` |
| Get Product By Id | finds the product created above |
| Update Product | the new price is saved |
| Delete Product | returns 204 |
| Get Deleted Product | returns 404, so the product is really gone |
| Create Invalid Product | returns 400 with the invalid fields |

## Run it from the command line

```powershell
npm install -g @usebruno/cli@4.2.0
cd api-tests
bru run --env Local
```

`bru run` exits with `0` when every check passes and `1` when any check fails, so it can fail a CI build.

## CI

`.github/workflows/api-tests.yml` builds the API, starts it on `http://localhost:5080`, waits until it answers and runs the collection with the same **Local** environment. A failed check fails the job.
