import { NgModule } from '@angular/core';
import { RouterModule, Routes, UrlSegment } from '@angular/router';
import { ViewComponent } from './view/view.component';

const routes: Routes = [
   { path: '', redirectTo: '/view', pathMatch: 'full' },
   { path: 'view', component: ViewComponent },
   { matcher: (url) => {
         // match urls like "/files/:filepath" where filepath can contain '/'
         if (url.length > 0) {
            // if first segment is 'files', then concat all the next segments into a single one
            // and return it as a parameter named 'filepath'
            if (url[0].path == "view") {
               var id = url.slice(1).join("/");
               return {
                  consumed: url,
                  posParams: {
                     id: new UrlSegment(id, {})
                  }
               };
            }
         }
         return null;
      }, component: ViewComponent },
   { path: 'home', component: ViewComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
