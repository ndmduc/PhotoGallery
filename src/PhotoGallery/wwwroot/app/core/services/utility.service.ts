import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class UtilityService {
    private _route: Router;

    constructor(router: Router) {
        this._route = router;
    }

    convertDateTime(date: Date) {
        var formattedDate = new Date(date.toString());
        return formattedDate.toDateString();
    }

    navigate(path: string) {
        this._route.navigate([path]);
    }

    navigateToSignIn() {
        this.navigate('/account/login');
    }
}