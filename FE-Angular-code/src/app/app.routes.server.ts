import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  {
    path: 'cryptoservice/user/:email',
    renderMode: RenderMode.Server
  },
  {
    path: 'Usersetting/:email',
    renderMode: RenderMode.Server
  },
  {
    path: 'registerservice/:serviceid',
    renderMode: RenderMode.Server
  },
  {
    path: '**',
    renderMode: RenderMode.Prerender
  }
];
