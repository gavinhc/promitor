﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Promitor.Agents.Scraper.Configuration;
using Promitor.Agents.Scraper.Validation.Interfaces;

namespace Promitor.Agents.Scraper.Validation.Steps.Sinks
{
    public class StatsDMetricSinkValidationStep : ValidationStep, IValidationStep
    {
        private readonly IOptions<ScraperRuntimeConfiguration> _runtimeConfiguration;

        public StatsDMetricSinkValidationStep(IOptions<ScraperRuntimeConfiguration> runtimeConfiguration)
            : this(runtimeConfiguration, NullLogger.Instance)
        {
        }

        public StatsDMetricSinkValidationStep(IOptions<ScraperRuntimeConfiguration> runtimeConfiguration, ILogger validationLogger)
            : base(validationLogger)
        {
            _runtimeConfiguration = runtimeConfiguration;
        }

        public string ComponentName { get; } = "StatsD Metric Sink";

        public ValidationResult Run()
        {
            var currentRuntimeConfiguration = _runtimeConfiguration.Value;
            var statsDConfiguration = currentRuntimeConfiguration?.MetricSinks?.Statsd;
            if (statsDConfiguration == null)
            {
                return ValidationResult.Successful(ComponentName);
            }

            if (string.IsNullOrWhiteSpace(statsDConfiguration.Host))
            {
                var errorMessage = "No host of StatsD server is configured";
                return ValidationResult.Failure(ComponentName, errorMessage);
            }

            if (statsDConfiguration.Port <= 0)
            {
                var errorMessage = $"StatsD port {statsDConfiguration.Port} is not allowed";
                return ValidationResult.Failure(ComponentName, errorMessage);
            }

            return ValidationResult.Successful(ComponentName);
        }
    }
}
