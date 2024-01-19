import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ChatGeneratorComponent } from './chat-generator.component';
import { GenerateHistoryComponent } from './generate-history/generate-history.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'history'
  },
  {
    path: 'history',
    component: GenerateHistoryComponent
  },
  {
    path: 'generate',
    component: ChatGeneratorComponent
  },
  {
    path: 'generate/:id',
    component: ChatGeneratorComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GenChatRoutingModule { }
