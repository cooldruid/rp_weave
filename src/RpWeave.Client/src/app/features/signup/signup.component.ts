import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from "@angular/material/card";
import { MatInputModule } from "@angular/material/input";
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { SignupService } from './signup.service';
import { MatButtonModule } from '@angular/material/button';

@Component({
    selector: 'app-signup',
    imports: [MatCardModule, MatInputModule, FormsModule, MatButtonModule],
    templateUrl: './signup.component.html',
    styleUrl: './signup.component.scss'
})
export class SignupComponent {
    username = '';
    password = '';
    confirmPassword = '';

    constructor(
        private router: Router, 
        private snackBar: MatSnackBar,
        private signupService: SignupService)
    { }

    async signUp() {
        try {
            if(this.password != this.confirmPassword){
                this.snackBar.open("Passwords do not match.");
                return;
            }

            await this.signupService.signUpAsync(this.username, this.password);

            this.router.navigate(['campaigns'])
        }
        catch(error: any) {
            this.snackBar.open(error.error, 'OK');
        }
    }

    cancelSignUp() {
        this.router.navigate(['']);
    }
}
