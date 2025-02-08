global program_с_b_main
 
section .data
	nums dq 10, 20, 30, 15, 15
	count equ ($-nums)/numSize    ; количество элементов
	numSize equ 8   ; размер каждого элемента

section .text
program_с_b_main:

	mov rdi, "X"
	push rdi

	mov rdi, 0xB8000
	add rdi, 4
	push rdi

	call write_vga_char
	add rsp, 4
	add rsp, 4

	ret

	;mov rdi, 11       ; в RDI параметр для функции sum
	;call sum            ; после вызова в RAX - результат сложения
	;mov rdi, rax     ; помещаем результат в RDI
	;ret
 
sum:
	push rbp              ; сохраняем старое значение RBP в стек
	mov rbp, rsp         ; копируем текущий адрес из RSP в RBP
	sub rsp, 16          ; выделяем место для двух переменных по 8 байт
 
	mov qword[rbp-8] , 7      ; По адресу [rbp-8] первая локальная переменная, равная 7
	mov qword [rbp-16], rdi    ; По адресу [rbp-16] вторая локальная переменная, равная RDI
 
	mov rax, [rbp-8]    ; в RAX значение из [rbp-8]  - первая локальная переменная
	add rax, [rbp-16]    ; RAX = RAX + [rbp-16] - вторая локальная переменная
 
	mov rsp, rbp         ; восстанавливаем ранее сохраненное значение RSP 
	pop rbp               ; восстанавливем RBP
	 
	ret


write_vga_char:

	; Openning
	;push rbp
	;mov rbp, rsp

	; Function's body

	mov rbx, [rsp+4]

	mov rcx, [rsp+8]

	mov byte [rbx], cl
	mov byte [rbx+1], 0x0F

	; Ending
	;mov rsp, rbp
	;pop rbp

	ret
