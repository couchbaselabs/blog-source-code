import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class GeosearchBox {
    public markers: any[];
    public latitudeTopLeft: any;
    public longitudeTopLeft: any;
    public latitudeBottomRight: any;
    public longitudeBottomRight: any;
    public http: HttpClient;

    constructor(http: HttpClient) {
        this.http = http;
        this.markers = [];

/*
        this.markers = [
            {
                latitude: -27.451673,   // brisbane
                longitude: 153.043981,
                infoWindow: { content: `<p>somewhere in Brisbane</p>`}
            },
            {
                latitude: 37.754582,    // sf
                longitude: -122.446418,
                infoWindow: { content: `<p>somewhere in sf</p>` }
            }
        ];
        */
    }

    public clickMap(event : any) {
        var latLng = event.detail.latLng,
            lat = latLng.lat(),
            lng = latLng.lng();

        // only update top left if it hasn't been set yet, or bottom right is already set
        if (!this.longitudeTopLeft || this.longitudeBottomRight) {
            this.longitudeTopLeft = lng;
            this.latitudeTopLeft = lat;
            this.longitudeBottomRight = null;
            this.latitudeBottomRight = null;
        } else {
            this.longitudeBottomRight = lng;
            this.latitudeBottomRight = lat;
        }
    }

    public searchClick() {
        console.log("Searching with bounding box (" + this.latitudeTopLeft + "," + this.longitudeTopLeft + ") - (" + this.latitudeBottomRight + ", " + this.longitudeBottomRight + ")");

        let boxSearch = {
            latitudeTopLeft: this.latitudeTopLeft,
            longitudeTopLeft: this.longitudeTopLeft,
            latitudeBottomRight: this.latitudeBottomRight,
            longitudeBottomRight: this.longitudeBottomRight
        };

        console.log("POSTing to api/Box: " + JSON.stringify(boxSearch));

        this.http.fetch('api/Box', { method: "POST", body: json(boxSearch) })
            .then(result => result.json() as Promise<any[]>)
            .then(data => {
                this.markers = data;
            });
    }
}

