import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Cliente, ClienteService } from '../../core/services/cliente.service';

@Component({
  selector: 'app-clientes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './clientes.html',
  styleUrls: ['./clientes.css']
})
export class Clientes {
  private service = inject(ClienteService);
  private fb = inject(FormBuilder);

  clientes: Cliente[] = [];
  editando = false;

  form = this.fb.group({
    id: [0],
    nome: ['', Validators.required],
    endereco: ['', Validators.required],
    telefone: ['', Validators.required],
  });

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe((data) => {
      this.clientes = data;
    });
  }

  salvar() {
    const cliente = this.form.value as Cliente;

    if (this.editando) {
      this.service.update(cliente).subscribe(() => {
        this.cancelar();
        this.load();
      });
    } else {
      this.service.create(cliente).subscribe(() => {
        this.form.reset();
        this.load();
      });
    }
  }

  editar(cliente: Cliente) {
    this.editando = true;
    this.form.patchValue(cliente);
  }

  excluir(id: number) {
    if (confirm('Deseja excluir este cliente?')) {
      this.service.delete(id).subscribe(() => {
        this.load();
      });
    }
  }

  cancelar() {
    this.editando = false;
    this.form.reset();
  }
}