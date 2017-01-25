angular.module('starter.controllers')
    .factory('globalConfigurationService', function () {
        return {
            googleMapsKey: 'AIzaSyDPeGZCk08qRieU44FKP0zN_Xq8yci6pIo',

            backendApiBaseUrl: 'http://vps96817.vps.ovh.ca/api'
        }
    });