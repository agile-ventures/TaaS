import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { SignalRService } from './signalr.service';
import { HttpClientModule } from '@angular/common/http';
import {
  MatInputModule,
  MatButtonModule,
  MatSnackBarModule,
  MatTableModule,
  MatPaginatorModule,
  MatCardModule,
  MatDividerModule,
  MatProgressBarModule,
  MatTabsModule,
  MatDialogModule
} from '@angular/material';
import { ServiceUrlDialogComponent } from './serviceurl-dialog/serviceurl-dialog.component';
import { AmountToTezPipe, FeeToTezPipe } from './pipes';

@NgModule({
  declarations: [
    AppComponent,
    ServiceUrlDialogComponent,
    AmountToTezPipe,
    FeeToTezPipe
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatTableModule,
    MatPaginatorModule,
    MatCardModule,
    MatDividerModule,
    MatProgressBarModule,
    MatTabsModule,
    MatDialogModule,
    ReactiveFormsModule
  ],
  providers: [
    SignalRService
  ],
  bootstrap: [AppComponent],
  entryComponents: [ServiceUrlDialogComponent]
})
export class AppModule { }
