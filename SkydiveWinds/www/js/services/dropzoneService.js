angular.module('starter.controllers')
    .factory('dropzoneService', function ($http, globalConfigurationService) {
        var dropzone = null;

        return {
            getDropzone: function(position, nameFilter) {
                var url = globalConfigurationService.backendApiBaseUrl + "/dropzones/nearest?name=" + nameFilter;
                if (position != null)
                    url += "&longitude=" + position.coords.longitude + "&latitude=" + position.coords.latitude;

                return $http.get(url, { timeout: globalConfigurationService.backendCallsTimeout }).then(function (response) {
                    dropzone = response.data;
                    return dropzone;
                }, function(error) {
                    //there was an error fetching from the server
                });
            }
        }
    });

