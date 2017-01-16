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

.factory('weatherService', function ($http) {
    var weather = [];

    return {
        getWeather: function (latitude, longitude) {
            return $http.get("http://vps96817.vps.ovh.ca/api/weather?longitude=" + longitude + "&latitude=" + latitude).then(function (response) {
                weather = response.data;
                return weather;
            }, function (error) {
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

.controller('LocationController', function ($scope, $ionicLoading, $ionicPopup, dropzoneService) {
    var getDropzoneFromService = function(position, nameFilter) {
        dropzoneService.getDropzone(position, nameFilter).then(function (dropzone) {
            $scope.locationInfo.latitude = dropzone.latitude;
            $scope.locationInfo.longitude = dropzone.longitude;
            $scope.locationInfo.dropzoneInfo = dropzone.address;
            $scope.locationInfo.dropzoneName = dropzone.name;

            $ionicLoading.hide();
        });
    };

    $scope.resetDropzone = function() {
        $scope.locationInfo.dropzoneName = null;
        $scope.locationInfo.dropzoneInfo = null;
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

        var storage = window.localStorage;
        storage.setItem('locationName', $scope.locationInfo.dropzoneName == null ? '(' + $scope.locationInfo.latitude + '; ' + $scope.locationInfo.longitude + ')' : $scope.locationInfo.dropzoneName);
        storage.setItem('latitude', $scope.locationInfo.latitude.toString());
        storage.setItem('longitude', $scope.locationInfo.longitude.toString());
    };

    $scope.locationInfo = {
        dropzoneSearchPattern: ''
    };
})

.controller('WeatherController', function ($scope, $location, $ionicLoading, weatherService) {
    $scope.loadWeather = function () {
        $ionicLoading.show({
            template: '<p>Loading...</p><ion-spinner></ion-spinner>'
        });

        weatherService.getWeather(44.238, -79.641).then(function (weather) {
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

            var windsAloftRecords = weather.windsAloft.windsAloftRecords;
            for (var i = 0; i < windsAloftRecords.length; i++) {
                windsAloftRecords[i].bearing = formatBearing(windsAloftRecords[i].windHeading);
            }
            weather.groundWeather.bearing = formatBearing(weather.groundWeather.windHeading);

            $scope.windsAloftRecords = windsAloftRecords;
            $scope.groundWeather = weather.groundWeather;

            $ionicLoading.hide();
        });
    }

    var storage = window.localStorage;
    $scope.locationName = storage.getItem('locationName');
    $scope.latitude = storage.getItem('latitude');
    $scope.longitude = storage.getItem('longitude');
    if ($scope.locationName === null || $scope.latitude === null || $scope.longitude == null) {
    }

    $scope.loadWeather();
});
