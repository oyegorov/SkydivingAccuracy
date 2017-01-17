angular.module('starter.controllers')
    .factory('unitsService', function() {
        var TEMP_CELSIUS = 'C';
        var TEMP_FAHRENHEIT = 'F';
        var WS_KMPH = 'km/h';
        var WS_MPS = 'm/s';
        var WS_MPH = 'mph';
        var WS_KNOTS = 'knots';
        var ALT_FEET = 'feet';
        var ALT_METERS = 'meters';

        return {
            getDefaultUnits() {
                return {
                    'temperatureUnits': TEMP_CELSIUS,
                    'windSpeedUnits': WS_MPH,
                    'altitudeUnits': ALT_FEET
                };
            },

            getTemperatureUnits: function() {
                return [TEMP_CELSIUS, TEMP_FAHRENHEIT];
            },

            getWindSpeedUnits: function() {
                return [WS_KMPH, WS_MPS, WS_MPH, WS_KNOTS];
            },

            getAltitudeUnits: function() {
                return [ALT_FEET, ALT_METERS];
            },

            convertTemperature: function(value, toUnits) {
                switch (toUnits) {
                case TEMP_FAHRENHEIT:
                    return value * 9 / 5 + 32;
                default:
                    return value;
                }
            },

            convertWindSpeed: function(value, toUnits) {
                switch (toUnits) {
                case WS_KMPH:
                    return value * 1.852;
                case WS_MPS:
                    return value * 0.514444;
                case WS_MPH:
                    return value * 1.151;
                default:
                    return value;
                }
            },

            convertAltitude: function(value, toUnits) {
                switch (toUnits) {
                case ALT_METERS:
                    return value / 3;
                default:
                    return value;
                }
            }
        }
    });