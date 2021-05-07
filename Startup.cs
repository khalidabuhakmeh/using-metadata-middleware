using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UsingMetadata
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ExtraMiddleware>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseMiddleware<ExtraMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/",
                    async context => { await context.Response.WriteAsync("Hello World!"); }
                ).WithMetadata(new Extra("X-Twitter", "@jchannon"));
            });
        }
    }

    public class ExtraMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();
            var metadata = endpoint?.Metadata.GetMetadata<Extra>();

            if (metadata is { })
            {
                context.Response.Headers.Add(metadata.Key, metadata.Value);
            }

            await next(context);
        }
    }

    public record Extra(string Key, string Value);
}