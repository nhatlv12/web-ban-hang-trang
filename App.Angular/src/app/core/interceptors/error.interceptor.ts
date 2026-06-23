import { HttpErrorResponse, HttpEvent, HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, map, throwError } from 'rxjs';
import { MessageService } from 'primeng/api';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);

  return next(req).pipe(
    map((event: HttpEvent<any>) => {
      if (event instanceof HttpResponse) {
        const body = event.body;
        // Kiểm tra ApiResponse format từ backend: { success: boolean, message: string, data: any }
        if (body && typeof body === 'object' && ('success' in body || 'sucess' in body || 'Success' in body)) {
          const isSuccess = body.success !== false && body.sucess !== false && body.Success !== false;
          if (!isSuccess) {
            throw new Error(body.message || body.Message || 'Lỗi từ máy chủ');
          }
        }
      }
      return event;
    }),
    catchError((error: any) => {
      let errorMsg = 'Có lỗi xảy ra trong quá trình xử lý, vui lòng thử lại sau.';
      let summary = 'Lỗi hệ thống';

      if (error instanceof HttpErrorResponse) {
        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMsg = error.error.message;
          summary = 'Lỗi ứng dụng';
        } else {
          // Server-side error
          summary = `Lỗi ${error.status}`;
          
          if (typeof error.error === 'string') {
            errorMsg = error.error;
          } else if (error.error?.message || error.error?.Message) {
            errorMsg = error.error.message || error.error.Message;
          } else if (error.error?.detail) {
            errorMsg = error.error.detail;
          } else if (error.error?.title) {
            errorMsg = error.error.title;
          } else if (error.message) {
            errorMsg = error.message;
          }
        }
      } else if (error instanceof Error) {
        // Lỗi do success: false trả về từ ApiResponse (HTTP 200)
        summary = 'Thao tác thất bại';
        errorMsg = error.message;
      }

      messageService.add({
        severity: 'error',
        summary: summary,
        detail: errorMsg,
        life: 5000
      });

      return throwError(() => error);
    })
  );
};
