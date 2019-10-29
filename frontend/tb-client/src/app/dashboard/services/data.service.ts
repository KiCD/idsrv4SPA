import { WeatherForecast } from './../models/weather-forecast.model';
import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  constructor(private http: HttpClient) { }
  getData(): Observable<WeatherForecast[]> {
    const endpointUrl = `${environment.webApiEndPoint}/protecteddata`;
    const jsonHeader = new HttpHeaders({ 'Content-Type': 'application/json'});
    return this.http.get<WeatherForecast[]>(endpointUrl, {
      headers: jsonHeader,
    });
  }
}
