﻿using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

// ReSharper disable UnusedMemberInSuper.Global

namespace FastEndpoints;

/// <summary>
/// the common interface implemented by all endpoints
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// the http context of the current request
    /// </summary>
    HttpContext HttpContext { get; } //this is for allowing consumers to write extension methods

    /// <summary>
    /// validation failures collection for the endpoint
    /// </summary>
    List<ValidationFailure> ValidationFailures { get; } //also for extensibility

    /// <summary>
    /// gets the endpoint definition which contains all the configuration info for the endpoint
    /// </summary>
    EndpointDefinition Definition { get; } //also for extensibility

    /// <summary>
    /// retrieves the name of a given endpoint by supplying its type. the name is generated using the <see cref="EndpointOptions.NameGenerator" /> func.
    /// </summary>
    /// <param name="verb">the http verb, if the target is a multi-verb endpoint.</param>
    /// <param name="routeNumber">the route number, if the target is a multi route endpoint.</param>
    /// <param name="tagPrefix">tag prefix</param>
    public static string GetName<TEndpoint>(Http? verb = null, int? routeNumber = null, string? tagPrefix = null) where TEndpoint : IEndpoint
        => Cfg.EpOpts.NameGenerator(new(typeof(TEndpoint), verb?.ToString("F"), routeNumber, tagPrefix));

    //key: the type of the endpoint
    static ConcurrentDictionary<Type, string> TestUrlCache { get; } = new();

    internal static void SetTestUrl(Type endpointType, string url)
        => TestUrlCache[endpointType] = url;

    //don't change to internal. this is unofficially exposed to public.
    public static string TestURLFor<TEndpoint>()
        => TestUrlCache[typeof(TEndpoint)];
}

/// <summary>
/// marker interface for endpoint base classes without a request dto
/// </summary>
public interface INoRequest;