import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
    selector: 'app-serviceurl-dialog',
    templateUrl: './serviceurl-dialog.component.html'
})

export class ServiceUrlDialogComponent implements OnInit {

    form: FormGroup;
    endpoint: string;

    constructor(
        private fb: FormBuilder,
        private dialogRef: MatDialogRef<ServiceUrlDialogComponent>,
        @Inject(MAT_DIALOG_DATA) { endpoint }: EndpointConfig) {
            this.endpoint = endpoint;
            this.form = fb.group({
                endpoint: [endpoint, Validators.required],
            });
         }

    ngOnInit() { }

    close() {
        this.dialogRef.close(this.form.value.endpoint);
    }
}

export interface EndpointConfig {
    endpoint: string;
}
