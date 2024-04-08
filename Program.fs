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

let get () = Option<string>.None

let get2 () = Option<int>.None

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