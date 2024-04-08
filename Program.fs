open System
// FSharp.SystemTextJson adds its stuff here
open System.Globalization
open System.Text.Json.Serialization
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Hosting.Server
open Microsoft.AspNetCore.Hosting.Server.Features
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
// Lots of stuff gets added under this namespace
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Http.Json
// This is an experimental library that tells the openapi stuff about fsharp types, so e.g. `T option` values are
// converted to nullable `T`s
open FSharp.SystemTextJson.Swagger
open Microsoft.OpenApi.Models

(*
Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware[1]
      An unhandled exception has occurred while executing the request.
      Swashbuckle.AspNetCore.SwaggerGen.SwaggerGeneratorException: Failed to generate Operation for action - HTTP: GET /sub => Invoke. See inner exception
       ---> Swashbuckle.AspNetCore.SwaggerGen.SwaggerGeneratorException: Failed to generate schema for type - Microsoft.FSharp.Core.FSharpResult`2[System.String,System.String]. See inner exception
       ---> System.InvalidOperationException: Can't use schemaId "$Ok" for type "$Ok". The same schemaId is already used for type "$Ok"
         at Swashbuckle.AspNetCore.SwaggerGen.SchemaRepository.RegisterType(Type type, String schemaId)
         at Swashbuckle.AspNetCore.SwaggerGen.SchemaGenerator.GenerateReferencedSchema(DataContract dataContract, SchemaRepository schemaRepository, Func`1 definitionFactory)
         at Swashbuckle.AspNetCore.SwaggerGen.SchemaGenerator.GenerateConcreteSchema(DataContract dataContract, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SchemaGenerator.<>c__DisplayClass9_0.<GeneratePolymorphicSchema>b__0(DataContract allowedTypeDataContract)
         at System.Linq.Utilities.<>c__DisplayClass2_0`3.<CombineSelectors>b__0(TSource x)
         at System.Linq.Enumerable.SelectEnumerableIterator`2.ToList()
         at System.Linq.Enumerable.ToList[TSource](IEnumerable`1 source)
         at Swashbuckle.AspNetCore.SwaggerGen.SchemaGenerator.GeneratePolymorphicSchema(DataContract dataContract, SchemaRepository schemaRepository, IEnumerable`1 knownTypesDataContracts)
         at Swashbuckle.AspNetCore.SwaggerGen.SchemaGenerator.GenerateSchemaForType(Type modelType, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SchemaGenerator.GenerateSchema(Type modelType, SchemaRepository schemaRepository, MemberInfo memberInfo, ParameterInfo parameterInfo, ApiParameterRouteInfo routeInfo)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateSchema(Type type, SchemaRepository schemaRepository, PropertyInfo propertyInfo, ParameterInfo parameterInfo, ApiParameterRouteInfo routeInfo)
         --- End of inner exception stack trace ---
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateSchema(Type type, SchemaRepository schemaRepository, PropertyInfo propertyInfo, ParameterInfo parameterInfo, ApiParameterRouteInfo routeInfo)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.CreateResponseMediaType(ModelMetadata modelMetadata, SchemaRepository schemaRespository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.<>c__DisplayClass25_0.<GenerateResponse>b__2(String contentType)
         at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector, IEqualityComparer`1 comparer)
         at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateResponse(ApiDescription apiDescription, SchemaRepository schemaRepository, String statusCode, ApiResponseType apiResponseType)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateResponses(ApiDescription apiDescription, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateOperation(ApiDescription apiDescription, SchemaRepository schemaRepository)
         --- End of inner exception stack trace ---
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateOperation(ApiDescription apiDescription, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GenerateOperations(IEnumerable`1 apiDescriptions, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GeneratePaths(IEnumerable`1 apiDescriptions, SchemaRepository schemaRepository)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GetSwaggerDocumentWithoutFilters(String documentName, String host, String basePath)
         at Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenerator.GetSwaggerAsync(String documentName, String host, String basePath)
         at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
         at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)
*)

let get () = Result<int, string>.Error "test"

let get2 () = Result<string, string>.Error "test"

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    let jsonFSharpOptions = JsonFSharpOptions(JsonUnionEncoding.FSharpLuLike)

    builder.Services
        .Configure(fun (opt: JsonOptions) ->
            opt.SerializerOptions.Converters.Add(JsonFSharpConverter(jsonFSharpOptions)))
        .AddSwaggerForSystemTextJson(
            jsonFSharpOptions,
            _.SwaggerDoc("v1", OpenApiInfo(Version = "v1", Title = "TODO API"))
        )
        .AddEndpointsApiExplorer()
    |> ignore

    let app = builder.Build()

    if app.Environment.IsDevelopment() then
        app.UseSwagger().UseSwaggerUI() |> ignore

    app.MapGet("/", Func<_>(get)) |> ignore
    app.MapGet("/sub", Func<_>(get2)) |> ignore

    app.Run()

    0 // Exit code