angular.module('starter.controllers')
    .factory('settingsService', function(unitsService) {
        var DEFAULT_PREFERRED_ALTITUDE = 12000;
        var preferredExitAltitudeKey = 'preferredExitAltitudeKey';
        var locationInfoKey = 'locationInfo';
        var unitsSettingsKey = 'unitsSettings';
        var storage = window.localStorage;

        return {
            loadLocationInfo: function() {
                var serializedSettings = storage.getItem(locationInfoKey);
                if (serializedSettings != null)
                    return JSON.parse(serializedSettings);

                return null;
            },

            saveLocationInfo: function(locationInfo) {
                storage.setItem(locationInfoKey, JSON.stringify(locationInfo));
            },

            loadUnitsSettings: function() {
                var serializedSettings = storage.getItem(unitsSettingsKey);
                if (serializedSettings != null)
                    return JSON.parse(serializedSettings);

                return unitsService.getDefaultUnits();
            },

            saveUnitsSettings: function(unitsSettings) {
                storage.setItem(unitsSettingsKey, JSON.stringify(unitsSettings));
            },

            loadPreferredExitAltitude: function() {
                var preferredAltitudeStringValue = storage.getItem(preferredExitAltitudeKey);
                return preferredAltitudeStringValue == null ? DEFAULT_PREFERRED_ALTITUDE : parseInt(preferredAltitudeStringValue);
            },

            savePreferredExitAltitude: function (preferredExitAltitude) {
                storage.setItem(preferredExitAltitudeKey, preferredExitAltitude.toString());
            }
        }
    });