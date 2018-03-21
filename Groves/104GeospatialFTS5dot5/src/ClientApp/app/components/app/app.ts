import { Aurelia, PLATFORM } from 'aurelia-framework';
import { Router, RouterConfiguration } from 'aurelia-router';

export class App {
    router: Router;

    configureRouter(config: RouterConfiguration, router: Router) {
        config.title = 'GeospatialSearch';
        config.map([{
            route: [ '', 'home' ],
            name: 'home',
            settings: { icon: 'home' },
            moduleId: PLATFORM.moduleName('../home/home'),
            nav: true,
            title: 'Home'
        }, {
            route: 'counter',
            name: 'counter',
            settings: { icon: 'education' },
            moduleId: PLATFORM.moduleName('../counter/counter'),
            nav: true,
            title: 'Counter'
        }, {
            route: 'fetch-data',
            name: 'fetchdata',
            settings: { icon: 'th-list' },
            moduleId: PLATFORM.moduleName('../fetchdata/fetchdata'),
            nav: true,
            title: 'Fetch data'
        }, {
            route: 'geo-search-box',
            name: 'geosearchbox',
                settings: { icon: 'map-marker' },
                moduleId: PLATFORM.moduleName('../geosearchbox/geosearchbox'),
            nav: true,
            title: 'Geo Search (Bounding Box)'
            }, {
                route: 'geo-search-point',
                name: 'geosearchpoint',
                settings: { icon: 'map-marker' },
                moduleId: PLATFORM.moduleName('../geosearchpoint/geosearchpoint'),
                nav: true,
                title: 'Geo Search (Distance)'
            }
            ]);

        this.router = router;
    }
}
