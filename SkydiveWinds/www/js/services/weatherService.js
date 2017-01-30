angular.module('starter.controllers')
    .service('weatherService', function ($http, globalConfigurationService, windsService) {
        var service = this;

        var loadWeatherFromService = function (locationInfo, onLoadStart, onLoadCompleted) {
            var url = globalConfigurationService.backendApiBaseUrl + "/weather?longitude=" + locationInfo.longitude + "&latitude=" + locationInfo.latitude;

            if (onLoadStart)
                onLoadStart();
            return $http.get(url, { timeout: globalConfigurationService.backendCallsTimeout }).then(function (response) {
                var weatherData = response.data;

                var w = { locationInfo: locationInfo };
                if (weatherData != null) {
                    var windsAloftRecords = [];
                    w.requestedOn = weatherData.requestedOn;

                    if (weatherData.windsAloft != null && weatherData.windsAloft.windsAloftRecords != null) {
                        if (weatherData.windsAloft.airport != null) {
                            w.windsAloftAirport = weatherData.windsAloft.airport.name;
                            w.windsAloftSource = weatherData.windsAloft.source;
                            w.windsAloftUpdatedOn = weatherData.windsAloft.updatedOn;
                            w.windsAloftValidFrom = weatherData.windsAloft.validFrom;
                            w.windsAloftValidTo = weatherData.windsAloft.validTo;
                        } else {
                            w.windsAloftAirport = null;
                        }

                        windsAloftRecords = weatherData.windsAloft.windsAloftRecords;
                        for (var i = 0; i < windsAloftRecords.length; i++) {
                            windsAloftRecords[i].bearing = windsService.formatBearing(windsAloftRecords[i].windHeading, windsAloftRecords[i].windSpeed);
                        }
                    } else {
                        w.windsAloftAirport = null;
                        w.windsAloftSource = null;
                        w.windsAloftUpdatedOn = null;
                        w.windsAloftValidFrom = null;
                        w.windsAloftValidTo = null;
                    }

                    if (weatherData.groundWeather != null) {
                        weatherData.groundWeather.bearing = windsService.formatBearing(weatherData.groundWeather.windHeading, weatherData.groundWeather.windSpeed);
                        if (weatherData.groundWeather.metarStation != null) {
                            w.groundStationInfo = weatherData.groundWeather.metarStation.icaoCode;
                            if (weatherData.groundWeather.metarStation.city != null)
                                w.groundStationInfo += ' (' + weatherData.groundWeather.metarStation.city + ')';

                            w.groundStationDistance = weatherData.groundWeather.distanceToStation;
                        } else {
                            w.groundStationInfo = null;
                            w.groundStationDistance = null;
                        }

                    } else {
                        w.groundStationInfo = null;
                        w.groundStationDistance = null;
                    }

                    w.windsAloftRecords = windsAloftRecords;
                    w.groundWeather = weatherData.groundWeather;
                    w.weatherLoaded = true;
                } else {
                    w.weatherLoaded = false;
                    w.errorLoadingWeather = true;
                }

                service.weather = w;

                if (onLoadCompleted)
                    onLoadCompleted();

            }, function (error) {
                service.weather = {
                    weatherLoaded: false,
                    locationInfo: locationInfo,
                    errorLoadingWeather: true
                };

                if (onLoadCompleted)
                    onLoadCompleted();
            });
        }

        this.loadWeather = function (locationInfo, forceReload, onLoadStart, onLoadCompleted, onLoadNotNeeded) {
            if (locationInfo == null) {
                this.weather = {
                    weatherLoaded: false,
                    locationInfo: null
                };
            }

            if (this.weather != null &&
                this.weather.weatherLoaded &&
                this.weather.locationInfo != null &&
                this.weather.locationInfo.latitude == locationInfo.latitude &&
                this.weather.locationInfo.longitude == locationInfo.longitude &&
                !forceReload) {

                if (onLoadNotNeeded != null)
                    onLoadNotNeeded();
                
                return;
            }

            loadWeatherFromService(locationInfo, onLoadStart, onLoadCompleted);
        }

        this.recalculateUnits = function() {
            this.weather = angular.copy(this.weather);
        }

        this.weather = {
            weatherLoaded: false,
            locationInfo: null
        };
    });