import { HttpClient, json } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';

@inject(HttpClient)
export class GeosearchBox {
    public markers: any[];
    public latitude: any;
    public longitude: any;
    public distance: any;
    public http: HttpClient;

    constructor(http: HttpClient) {
        this.http = http;
        this.markers = [];
        this.distance = 10;

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

    public clickMap(event: any) {
        var latLng = event.detail.latLng,
            lat = latLng.lat(),
            lng = latLng.lng();

        // only update top left if it hasn't been set yet, or bottom right is already set
        this.longitude = lng;
        this.latitude = lat;
    }

    public searchClick() {
        console.log("Searching " + this.distance + " mile radius of (" + this.latitude + "," + this.longitude + ")");

        let pointSearch = {
            latitude: this.latitude,
            longitude: this.longitude,
            distance: this.distance
        };

        console.log("POSTing to api/Point: " + JSON.stringify(pointSearch));

        this.http.fetch('api/Point', { method: "POST", body: json(pointSearch) })
            .then(result => result.json() as Promise<any[]>)
            .then(data => {
                this.markers = data;
            });
    }
}

