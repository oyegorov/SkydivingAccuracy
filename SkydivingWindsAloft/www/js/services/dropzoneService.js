angular.module('starter.controllers', [])
    .factory('dropzoneService', function($http) {
        var dropzone = null;

        return {
            getDropzone: function(position, nameFilter) {
                var url = "http://vps96817.vps.ovh.ca/api/dropzones/nearest?name=" + nameFilter;
                if (position != null)
                    url += "&longitude=" + position.coords.longitude + "&latitude=" + position.coords.latitude;

                return $http.get(url, { timeout: 12000 }).then(function (response) {
                    dropzone = response.data;
                    return dropzone;
                }, function(error) {
                    //there was an error fetching from the server
                });
            }
        }
    });

