angular.module('starter.controllers', [])

.directive('rotate', function () {
    return {
        link: function (scope, element, attrs) {
            // watch the degrees attribute, and update the UI when it changes
            scope.$watch(attrs.degrees,
                function (rotateDegrees) {
                    console.log(rotateDegrees);
                    //transform the css to rotate based on the new rotateDegrees
                    element.css({
                        'transform': 'rotate(' + rotateDegrees + 'deg)',
                        '-moz-transform': 'rotate(' + rotateDegrees + 'deg)',
                        '-webkit-transform': 'rotate(' + rotateDegrees + 'deg)',
                        '-o-transform': 'rotate(' + rotateDegrees + 'deg)',
                        '-ms-transform': 'rotate(' + rotateDegrees + 'deg)'
                    });
                });
        }
    }
})

.factory('settingsService', function (unitsService) {
    var locationInfoKey = 'locationInfo';
    var unitsSettingsKey = 'unitsSettings';
    var storage = window.localStorage;

    return {
        loadLocationInfo: function () {
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

        saveUnitsSettings: function (unitsSettings) {
            storage.setItem(unitsSettingsKey, JSON.stringify(unitsSettings));
        }
    }
})

.factory('unitsService', function () {
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

        convertTemperature: function (value, toUnits) {
            switch (toUnits) {
                case TEMP_FAHRENHEIT:
                    return value * 9 / 5 + 32;
                default:
                    return value;
            }
        },

        convertWindSpeed: function (value, toUnits) {
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
})

.factory('weatherService', function ($http) {
    var weather = [];

    return {
        getWeather: function (latitude, longitude) {
            return $http.get("http://vps96817.vps.ovh.ca/api/weather?longitude=" + longitude + "&latitude=" + latitude).then(function (response) {
                weather = response.data;
                return weather;
            }, function (error) {
                return null;
                //there was an error fetching from the server
            });
        }
    }
})

.factory('dropzoneService', function ($http) {
    var dropzone = null;

    return {
        getDropzone: function (position, nameFilter) {
            var url = "http://vps96817.vps.ovh.ca/api/dropzones/nearest?name=" + nameFilter;
            if (position != null)
                url += "&longitude=" + position.coords.longitude + "&latitude=" + position.coords.latitude;

            return $http.get(url).then(function (response) {
                dropzone = response.data;
                return dropzone;
            }, function (error) {
                //there was an error fetching from the server
            });
        }
    }
})

.controller('SettingsController', function ($scope, settingsService, unitsService) {
    $scope.temperatureUnits = unitsService.getTemperatureUnits();
    $scope.windSpeedUnits = unitsService.getWindSpeedUnits();
    $scope.altitudeUnits = unitsService.getAltitudeUnits();

    $scope.onUnitsChanged = function() {
        settingsService.saveUnitsSettings($scope.unitsSettings);
    }

    $scope.$on('$ionicView.beforeEnter', function () {
        $scope.unitsSettings = settingsService.loadUnitsSettings();
    });
})

.controller('LocationController', function ($scope, $ionicLoading, $ionicPopup, $ionicHistory, $state, dropzoneService, settingsService) {
    var getDropzoneFromService = function(position, nameFilter) {
        dropzoneService.getDropzone(position, nameFilter).then(function (dropzone) {
            $scope.locationInfo.latitude = dropzone.latitude;
            $scope.locationInfo.longitude = dropzone.longitude;
            $scope.locationInfo.dropzoneInfo = dropzone.address;
            $scope.locationInfo.dropzoneName = dropzone.name;

            $scope.disableSaveButton = false;
            $ionicLoading.hide();
        });
    };

    $scope.onCoordinatesChanged = function() {
        $scope.locationInfo.dropzoneName = null;
        $scope.locationInfo.dropzoneInfo = null;
        $scope.disableSaveButton = false;
    };

    $scope.findDropzone = function () {
        $ionicLoading.show({
            template: '<p>Loading...</p><ion-spinner></ion-spinner>'
        });

        navigator.geolocation.getCurrentPosition(
            function(position) {
                getDropzoneFromService(position, $scope.locationInfo.dropzoneSearchPattern);
            },
            function(error) {
                getDropzoneFromService(null, $scope.locationInfo.dropzoneSearchPattern);
            },
            { enableHighAccuracy: true, timeout: 3000 });
    };

    $scope.saveChanges = function() {
        if (!$scope.locationInfo.latitude || !$scope.locationInfo.longitude) {
            $ionicPopup.alert({
                title: 'Save failed',
                template: 'Location was not specified.'
            });

            return;
        }

        settingsService.saveLocationInfo($scope.locationInfo);

        $ionicHistory.nextViewOptions({
            disableBack: true
        });

        $state.transitionTo('app.weather');
    };

    $scope.$on('$ionicView.beforeEnter', function () {
        var locationInfo = settingsService.loadLocationInfo();
        $scope.locationInfo = locationInfo != null
            ? locationInfo
            : {
                dropzoneSearchPattern: ''
            };
        $scope.disableSaveButton = true;
    });
})

.controller('WeatherController', function ($scope, $state, $ionicLoading, $ionicHistory, weatherService, settingsService, unitsService) {
    var loadWeather = function (latitude, longitude) {
        $ionicLoading.show({
            template: '<p>Loading...</p><ion-spinner></ion-spinner>'
        });

        weatherService.getWeather(latitude, longitude).then(function (weather) {
            function formatBearing(bearing) {
                if (bearing < 0 && bearing > -180) {
                    bearing = 360.0 + bearing;
                }
                if (bearing > 360 || bearing < -180) {
                    return "Unknown";
                }

                var directions = ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE",
                    "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW",
                    "N"];
                var cardinal = directions[Math.floor(((bearing + 11.25) % 360) / 22.5)];
                return bearing + "\u00b0 " + cardinal;
            };

            var windsAloftRecords = [];

            if (weather.windsAloft != null && weather.windsAloft.windsAloftRecords != null) {
                windsAloftRecords = weather.windsAloft.windsAloftRecords;
                for (var i = 0; i < windsAloftRecords.length; i++) {
                    windsAloftRecords[i].bearing = formatBearing(windsAloftRecords[i].windHeading);
                }
            }
            if (weather.groundWeather != null)
                weather.groundWeather.bearing = formatBearing(weather.groundWeather.windHeading);

            $scope.windsAloftRecords = windsAloftRecords;
            $scope.groundWeather = weather.groundWeather;
            $scope.weatherLoaded = true;            

            $ionicLoading.hide();
        });
    }
    
    $scope.reloadWeather = function() {
        var locationInfo = settingsService.loadLocationInfo();
        
        if (locationInfo != null) {
            if (locationInfo.dropzoneName != null)
                $scope.locationName = locationInfo.dropzoneName;
            else 
                $scope.locationName = '(' + locationInfo.latitude + '; ' + locationInfo.longitude + ')';
            
            loadWeather(locationInfo.latitude, locationInfo.longitude);

        } else {
            $scope.weatherLoaded = false;
            $scope.dropzoneName = null;
        }
    }

    $scope.gotoLocationSettings = function () {
        $ionicHistory.nextViewOptions({
            disableBack: true
        });
        
        $state.transitionTo('app.location');
    }

    $scope.$on('$ionicView.beforeEnter', function () {
        $scope.reloadWeather();
        $scope.unitsSettings = settingsService.loadUnitsSettings();
        $scope.unitsService = unitsService;
    });
});
