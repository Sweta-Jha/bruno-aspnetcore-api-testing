# Testing an ASP.NET Core API with Bruno on Windows

A small demo that shows how to test an ASP.NET Core Web API with the [Bruno](https://www.usebruno.com/) API client.

- `src/ProductCatalog.Api`: a .NET 10 Web API with one controller, `ProductsController` (create, read, update, delete)
- `api-tests`: the Bruno collection that tests it. Every request is a plain-text `.bru` file you can commit to Git.

## What you need

- Windows 10 or 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (`winget install Microsoft.DotNet.SDK.10`)
- [Bruno](https://www.usebruno.com/downloads) (`winget install Bruno.Bruno`)

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

| Request | What it checks |
|---|---|
| Get All Products | returns 200 and a list |
| Create Product | returns 201, saves the new id as `productId` |
| Get Product By Id | finds the product created above |
| Update Product | the new price is saved |
| Delete Product | returns 204 |
| Create Invalid Product | returns 400 with the invalid fields |

## Run it from the command line (optional)

```powershell
npm install -g @usebruno/cli
cd api-tests
bru run --env Local
```
