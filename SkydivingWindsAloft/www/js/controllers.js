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
            $scope.latitude = dropzone.latitude;
            $scope.longitude = dropzone.longitude;
            $scope.dropzoneInfo = dropzone.address;
            $scope.dropzoneName = dropzone.name;

            $ionicLoading.hide();
        });
    };

    $scope.dropzoneSearchPatternChange = function(obj) {
        $scope.dropzoneSearchPattern = obj.dropzoneSearchPattern;
    };

    $scope.resetDropzone = function() {
        $scope.dropzoneName = null;
        $scope.dropzoneInfo = null;
    };

    $scope.findDropzone = function () {
        $ionicLoading.show({
            template: '<p>Loading...</p><ion-spinner></ion-spinner>'
        });

        navigator.geolocation.getCurrentPosition(
            function(position) {
                getDropzoneFromService(position, $scope.dropzoneSearchPattern);
            },
            function(error) {
                getDropzoneFromService(null, $scope.dropzoneSearchPattern);
            },
            { enableHighAccuracy: true, timeout: 3000 });
    };

    $scope.saveChanges = function() {
        if (!$scope.latitude || !$scope.longitude) {
            $ionicPopup.alert({
                title: 'Could not save',
                template: 'Location was not specified.'
            });

            return;
        }

        var storage = window.localStorage;
        storage.setItem('locationName', $scope.dropzoneName == null ? '(' + $scope.latitude + '; ' + $scope.longitude + ')' : $scope.dropzoneName);
        storage.setItem('latitude', $scope.latitude.toString());
        storage.setItem('longitude', $scope.longitude.toString());
    };

    $scope.dropzoneSearchPattern = "";
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
