/* eslint-disable */
import { HttpClient, HttpContext, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { StrictHttpResponse } from '../../strict-http-response';
import { RequestBuilder } from '../../request-builder';

import { GuidApiResponse } from '../../models/guid-api-response';

export interface ApiOrdersIdPut$Params {
  id: string;
  body: any;
}

export function apiOrdersIdPut(http: HttpClient, rootUrl: string, params: ApiOrdersIdPut$Params, context?: HttpContext): Observable<StrictHttpResponse<GuidApiResponse>> {
  const rb = new RequestBuilder(rootUrl, apiOrdersIdPut.PATH, 'put');
  if (params) {
    rb.path('id', params.id, {});
    rb.body(params.body, 'application/json');
  }

  return http.request(
    rb.build({ responseType: 'json', accept: 'application/json', context })
  ).pipe(
    filter((r: any): r is HttpResponse<any> => r instanceof HttpResponse),
    map((r: HttpResponse<any>) => {
      return r as StrictHttpResponse<GuidApiResponse>;
    })
  );
}

apiOrdersIdPut.PATH = '/api/orders/{id}';
