# Testing an ASP.NET Core API with Bruno on Windows

A demo that shows how to test an ASP.NET Core Web API with the [Bruno](https://www.usebruno.com/) API client, from your first request to a CI pipeline.

- `src/ProductCatalog.Api`: a .NET 10 Web API with one controller, `ProductsController` (create, read, update, delete), protected by an API key
- `api-tests`: the Bruno collection that tests it. Every request is a plain-text `.bru` file you can commit to Git.
- `.github/workflows/api-tests.yml`: runs the collection in GitHub Actions on every push and pull request and saves an HTML report

The products are kept in memory to keep the demo focused on testing, so they reset when the API restarts. This is not a production pattern.

## What you need

- Windows 10 or 11
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (`winget install Microsoft.DotNet.SDK.10`)
- [Bruno](https://www.usebruno.com/downloads) (`winget install Bruno.Bruno`)
- [Node.js](https://nodejs.org/) 22, only for the Bruno CLI (`winget install OpenJS.NodeJS.LTS`)

Tested with .NET SDK 10.0.200, Bruno 4.2, Bruno CLI 4.2.0 and Node.js 22.14.0.

## Set up the API key

Every `/api` request must send an `X-Api-Key` header. The API and Bruno each need the same key, and neither copy is committed to Git.

```powershell
git clone https://github.com/Sweta-Jha/bruno-aspnetcore-api-testing.git
cd bruno-aspnetcore-api-testing

# 1. Give the key to the API (stored in your user profile, outside the repo)
dotnet user-secrets set ApiKey local-dev-key --project src\ProductCatalog.Api

# 2. Give the same key to Bruno (api-tests\.env is ignored by Git)
copy api-tests\.env.sample api-tests\.env
```

If you skip step 1, the API stops at startup with a message that tells you what to run.

## Run the API

```powershell
dotnet run --project src\ProductCatalog.Api
```

The API listens on `http://localhost:5080`.

## Test it with Bruno

1. Open Bruno and choose **Open Collection**.
2. Select the `api-tests` folder.
3. In the top-right environment list, pick **Local**.
4. Right-click the **Product API** collection and choose **Run** to run all requests.

The collection sets the API key once, in `api-tests/collection.bru`. Each request uses `auth: inherit`, so it sends that key without repeating it.

### Products folder: the life of one product

These run in order. **Create Product** must run first, because it saves the new id in the `productId` variable that the next requests use.

| Request | What it checks |
|---|---|
| Get All Products | returns 200 and the seeded list |
| Create Product | returns 201 and a `Location` header, saves the new id as `productId` |
| Get Product By Id | finds the product and has exactly the fields `id`, `name`, `price`, `stock` |
| Update Product | the new price and stock are saved |
| Delete Product | returns 204 |
| Get Deleted Product | returns 404, so the product is really gone |

### Error Cases folder: requests that should fail

These do not depend on each other or on the Products folder.

| Request | What it checks |
|---|---|
| Create Invalid Product | returns 400 and names both invalid fields |
| Update Missing Product | returns 404 for an id that does not exist |
| Missing API Key | returns 401 when the key is not sent |

Each request also has a **Docs** tab in Bruno that explains why it is there.

## Run it from the command line

```powershell
npm install -g @usebruno/cli@4.2.0
cd api-tests
bru run --env Local --reporter-html results/report.html
```

`bru run` exits with `0` when every check passes and `1` when any check fails, so it can fail a CI build. Open `results/report.html` in a browser to see each request, response and check.

## CI

`.github/workflows/api-tests.yml`:

1. builds the API;
2. creates a random API key for this run only and hides it in the logs;
3. starts the API with that key and waits until it answers;
4. runs the collection, which reads the same key from the `API_KEY` environment variable;
5. uploads the HTML and JUnit reports as the `bruno-report` artifact, even when a check fails.

A failed check fails the job.
