﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImGalaxy.ES.Core
{
    public static class ObjectExtensions
    {
        public static async Task<T> ThenAsync<T>(this T @obj, Func<T, Task> then)
        {
            await then(@obj);
            return @obj;
        }

        public static T Then<T>(this T @obj, Action<T> then)
        {
            then(@obj);
            return @obj;
        }

        public static async Task<T> ThrowsIfAsync<T>(this T @obj, Func<T, Task<bool>> assert, Exception exception)
        {
            var sut = await assert(@obj);

            if (sut)
                throw exception;

            return @obj;
        }

        public static T ThrowsIfNull<T>(this T @obj, Exception exception)
        {
            if (@obj == null)
                throw exception;

            return @obj;
        }

        public static T ThrowsIf<T>(this T @obj, Func<T, bool> assert, Exception exception)
        {
            var sut = assert(@obj);

            if (sut)
                throw exception;

            return @obj;
        }


        public static T ThrowsIf<T>(this T @obj, Func<T, bool> assert, Action<T> throwing)
        {
            var sut = assert(@obj);

            if (sut)
                throwing(@obj);

            return @obj;
        }

        public static async Task<T> ThrowsIfAsync<T>(this T @obj, Func<T, Task<bool>> assert, Func<T, Task> throwing)
        {
            var sut = await assert(@obj);

            if (sut)
                await throwing(@obj);

            return @obj;
        }
        public static T With<T>(this T @obj, Action<T> update)
        {
            update(@obj);
            return @obj;
        }
        public static async Task<T> WithAsync<T>(this T @obj, Func<T, Task> update)
        {
            await update(@obj);
            return @obj;
        }
        public static TDestination To<TSource, TDestination>(this TSource @obj, Func<TSource, TDestination> update) =>
            update(@obj);

        public static TDestination To<TSource, TDestination>(this TSource @obj, TDestination target) =>
           To(@obj, source => target);

        public static async Task<TDestination> ToAsync<TSource, TDestination>(this TSource @obj, Func<TSource, Task<TDestination>> update) =>
            await update(@obj);

        public static async Task<TDestination> ToAsync<TSource, TDestination>(this TSource @obj, TDestination target) =>
           await ToAsync(@obj, async source => target);
    }
}
