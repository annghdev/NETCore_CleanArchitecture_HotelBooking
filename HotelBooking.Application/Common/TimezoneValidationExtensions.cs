using FluentValidation;
using System;
using System.Linq;
using System.Reflection;

namespace HotelBooking.Application.Common;

/// <summary>
/// FluentValidation extensions for timezone handling
/// </summary>
public static class TimezoneValidationExtensions
{
    /// <summary>
    /// Validates DateTimeOffset to be valid
    /// </summary>
    /// <typeparam name="T">Validation target type</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder</param>
    /// <returns>Rule builder for chaining</returns>
    public static IRuleBuilderOptions<T, DateTimeOffset> MustBeVietnamTime<T>(
        this IRuleBuilder<T, DateTimeOffset> ruleBuilder)
    {
        return ruleBuilder
            .Must(BeValidDateTimeOffset)
            .WithMessage("Date and time must be valid");
    }

    /// <summary>
    /// Validates nullable DateTimeOffset to be valid
    /// </summary>
    /// <typeparam name="T">Validation target type</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder</param>
    /// <returns>Rule builder for chaining</returns>
    public static IRuleBuilderOptions<T, DateTimeOffset?> MustBeVietnamTime<T>(
        this IRuleBuilder<T, DateTimeOffset?> ruleBuilder)
    {
        return ruleBuilder
            .Must(BeValidNullableDateTimeOffset)
            .WithMessage("Date and time must be valid");
    }

    /// <summary>
    /// Validates DateTimeOffset to be in the future (Vietnam timezone)
    /// </summary>
    /// <typeparam name="T">Validation target type</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder</param>
    /// <param name="minimumMinutesFromNow">Minimum minutes from current Vietnam time</param>
    /// <returns>Rule builder for chaining</returns>
    public static IRuleBuilderOptions<T, DateTimeOffset> MustBeFutureVietnamTime<T>(
        this IRuleBuilder<T, DateTimeOffset> ruleBuilder, int minimumMinutesFromNow = 0)
    {
        return ruleBuilder
            .Must(value =>
            {
                var vietnamNow = TimeZoneHelper.ToVietnamTime(DateTimeOffset.UtcNow);
                var normalizedValue = TimeZoneHelper.EnsureVietnamTimeZone(value);
                return normalizedValue > vietnamNow.AddMinutes(minimumMinutesFromNow);
            })
            .WithMessage($"Date and time must be at least {minimumMinutesFromNow} minutes from current Vietnam time");
    }

    /// <summary>
    /// Validates DateTimeOffset range (From <= To) in Vietnam timezone
    /// </summary>
    /// <typeparam name="T">Validation target type</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder</param>
    /// <param name="getToValue">Function to get the 'To' value from the object</param>
    /// <returns>Rule builder for chaining</returns>
    public static IRuleBuilderOptions<T, DateTimeOffset> MustBeBeforeVietnamTime<T>(
        this IRuleBuilder<T, DateTimeOffset> ruleBuilder, Func<T, DateTimeOffset> getToValue)
    {
        return ruleBuilder
            .Must((obj, fromValue) =>
            {
                var toValue = getToValue(obj);
                var normalizedFrom = TimeZoneHelper.EnsureVietnamTimeZone(fromValue);
                var normalizedTo = TimeZoneHelper.EnsureVietnamTimeZone(toValue);
                return normalizedFrom <= normalizedTo;
            })
            .WithMessage("Start date must be before or equal to end date");
    }

    /// <summary>
    /// Validates that DateTimeOffset is within reasonable business hours (Vietnam timezone)
    /// </summary>
    /// <typeparam name="T">Validation target type</typeparam>
    /// <param name="ruleBuilder">FluentValidation rule builder</param>
    /// <param name="startHour">Start hour (default 6 AM)</param>
    /// <param name="endHour">End hour (default 11 PM)</param>
    /// <returns>Rule builder for chaining</returns>
    public static IRuleBuilderOptions<T, DateTimeOffset> MustBeWithinBusinessHours<T>(
        this IRuleBuilder<T, DateTimeOffset> ruleBuilder, int startHour = 6, int endHour = 23)
    {
        return ruleBuilder
            .Must(value =>
            {
                var vietnamTime = TimeZoneHelper.EnsureVietnamTimeZone(value);
                var hour = vietnamTime.Hour;
                return hour >= startHour && hour <= endHour;
            })
            .WithMessage($"Time must be between {startHour:D2}:00 and {endHour:D2}:59 Vietnam time");
    }

    private static bool BeValidDateTimeOffset(DateTimeOffset value)
    {
        return value != DateTimeOffset.MinValue && value != DateTimeOffset.MaxValue;
    }

    private static bool BeValidNullableDateTimeOffset(DateTimeOffset? value)
    {
        return !value.HasValue || BeValidDateTimeOffset(value.Value);
    }
}

/// <summary>
/// Request normalization extensions for timezone handling
/// </summary>
public static class TimezoneRequestExtensions
{
    /// <summary>
    /// Normalizes all DateTimeOffset properties in a request to Vietnam timezone
    /// </summary>
    /// <typeparam name="T">Request type</typeparam>
    /// <param name="request">Request object</param>
    /// <returns>Request with normalized timezone</returns>
    public static T NormalizeVietnamTimezone<T>(this T request) where T : class?
    {
        if (request == null) return request;

        try
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?))
                .Where(p => p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                var currentValue = property.GetValue(request);
                
                if (currentValue is DateTimeOffset dateTimeOffset)
                {
                    property.SetValue(request, TimeZoneHelper.EnsureVietnamTimeZone(dateTimeOffset));
                }
                else if (currentValue != null && property.PropertyType == typeof(DateTimeOffset?))
                {
                    var nullableValue = (DateTimeOffset?)currentValue;
                    if (nullableValue.HasValue)
                    {
                        property.SetValue(request, TimeZoneHelper.EnsureVietnamTimeZone(nullableValue.Value));
                    }
                }
            }
        }
        catch (Exception)
        {
            // Safely ignore reflection errors - return original request
            // This prevents breaking the application if reflection fails
        }

        return request;
    }
}
