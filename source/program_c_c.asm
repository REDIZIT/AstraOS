global program_с_c_main
global print_string_stack_coord_locals

section .data
	nums dq 10, 20, 30, 15, 15
	count equ ($-nums)/numSize    ; количество элементов
	numSize equ 8   ; размер каждого элемента
	width dq 80
	width_doubled dq 160
	test_str dd "Hello world"
	test_str2 dd "Second string"


section .text
program_с_c_main:

	mov rdi, "X"
	push rdi

	mov rdi, 0xB8000
	add rdi, 4
	push rdi

	call write_vga_char
	add rsp, 4
	add rsp, 4



	;mov rdi, test_str2
	;push rdi

	push test_str2

	;mov rdi, 0xB8000
	;add rdi, 10
	;push rdi

	;call print_string_stack
	;add rsp, 8

	push 5
	push 0

	;call print_string_stack_coord
	;add rsp, 12

	call print_string_stack_coord_locals
	add rsp, 12

	ret


write_vga_char:
	mov rbx, [rsp+4]

	mov rcx, [rsp+8]

	mov byte [rbx], cl
	mov byte [rbx+1], 0x0F

	ret


; rsi - pointer to string
; rdi - pointer to VGA cell
; used: rcx, al, rdi
print_string:
	mov rcx, -1
.print_loop:
	lodsb
	test al, al
	jz .done
	mov [rdi], al
	mov byte [rdi+1], 0x07
	add rdi, 2
	loop .print_loop
.done:
	ret


; 1: pointer to string
; 2: pointer to VGA cell
; used: rcx, al, rdi
print_string_stack:
	mov rcx, -1
	mov rsi, [rsp+8]
	mov rdi, [rsp+4]
.print_loop:
	lodsb
	test al, al
	jz .done
	mov [rdi], al
	mov byte [rdi+1], 0x0f
	add rdi, 2
	loop .print_loop
.done:
	ret


; 1: (+12) pointer to string
; 2: (+8) x coord
; 3: (+4) y coord
; used: rcx, al, rdi, rax
print_string_stack_coord:
	mov rsi, [rsp+12]
	mov rdi, [rsp+8]
	imul rdi, 2
	add rdi, 0xB8000

	mov rdx, [rsp+4]
	imul rdx, [width_doubled]
	add rdi, rdx

.print_loop:
	lodsb
	test al, al
	jz .done
	mov [rdi], al
	mov byte [rdi+1], 0x0f
	add rdi, 2
	loop .print_loop
.done:
	ret


; 1: (+12) int ptr_string - pointer to string
; 2: (+8) int x - x coord
; 3: (+4) int y - y coord
; used: rcx, al, rdi, rax
print_string_stack_coord_locals:

	; int ptr_vga_cell = 0xB8000 + (x + y * width) * 2
	; =>

	; temp - rbx
	; [rsp-4] - ptr_vga_cell

	; temp = y * width
	mov rcx, [rsp+4]
	mov rdx, [width]
	imul rcx, rdx
	mov rbx, rcx

	; temp = x + temp
	mov rcx, [rsp+8]
	mov rdx, rbx
	add rcx, rdx
	mov rbx, rcx

	; temp = temp * 2
	mov rcx, rbx
	mov rdx, 2
	imul rcx, rdx
	mov rbx, rcx

	; temp = 0xB8000 + temp
	mov rcx, 0xB8000
	mov rdx, rbx
	add rcx, rdx
	mov rbx, rcx

	; ptr_vga_cell = temp
	mov [rsp-4], rbx

	mov rdi, [rsp-4]  ; rdi = ptr_vga_cell
	mov rsi, [rsp+12] ; rsi = arg[0] (ptr_string)

.print_loop:
	lodsb
	test al, al
	jz .done
	mov [rdi], al
	mov byte [rdi+1], 0x0f
	add rdi, 2
	loop .print_loop
.done:
	ret
