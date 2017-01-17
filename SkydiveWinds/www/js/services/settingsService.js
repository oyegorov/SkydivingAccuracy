angular.module('starter.controllers')
    .factory('settingsService', function(unitsService) {
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
            }
        }
    });