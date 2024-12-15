global program_d_main
extern print_string_stack_coord_locals

global int_to_string
global reverse_string
global int_to_hex_string
global int_to_bin_string

section .data
	const2:     dd 2
	const10:    dd 10
	const16:    dd 16
	hex_digits dd '0123456789ABCDEF'

section .bss
	buffer resb 32


section .text
program_d_main:

	push 12345
	push buffer
	call int_to_string
	add rsp, 8

	push buffer  ; "12345"
	push 0
	push 0
	call print_string_stack_coord_locals
	add rsp, 12

	push buffer
	call reverse_string
	add rsp, 4

	push buffer  ; "54321"
	push 0
	push 1
	call print_string_stack_coord_locals
	add rsp, 12

	ret



; Input
; (+8) eax - number
; (+4) rcx - ptr to buffer
int_to_string:

	mov edx, 0
	mov rbx, 0

	mov eax, [rsp+8]
	add eax, 1  ; MAGIC NUMBER. Somewhy if you use "mov [rcx+rbx], dl" instead of "mov [buffer+rbx], dl" you will get number exactly on 1 less. This "add" "fix" that.

	mov rcx, [rsp+4]

int_to_string_loop:
	; Divide EAX by 10
	; div - Unsigned divide EDX:EAX by r/m32, with result stored in EAX := Quotient, EDX := Remainder

	div dword [const10]

	push rax

	; Convert int (digit) to ASCII char
	add edx, '0'

	; Move char to buffer
	mov [rcx+rbx], dl
	add rbx, 1

	pop rax
	mov rdx, 0

	; if remainder - EDX == 0: quit, else: recursive
	test eax, eax
	je .int_to_string_exit
	; else
	jmp int_to_string_loop
.int_to_string_exit:
	; then
	push rcx
	call reverse_string
	add rsp, 4

	ret


; input
; (+4) rsi - pointer to source string
reverse_string:
	mov rbx, 0  ; len
	mov rsi, [rsp+4]

.get_len:
	mov rcx, [rsi+rbx]  ; string[rbx]
	test rcx, rcx
	je .l1  ; if char is 0?
	; else
	add rbx, 1
	jmp .get_len
.l1:
	; then
	jmp .swap

.swap:
	; rax - 
	; rbx - len => j
	; rcx - i
	; rdx - temp
	; rsi, rdi - pointers

	sub rbx, 1  ; j
	mov rcx, 0  ; i

	; while (i < j)
.while:
	cmp rbx, rcx
	jng .end  ; if (i < j)
	; body

	; temp = str[i]
	mov byte al, [rsi+rcx]

	; str[i] = str[j]
	mov byte dl, [rsi+rbx]
	mov byte [rsi+rcx], dl

	; str[j] = temp
	mov byte [rsi+rbx], al


	add rcx, 1
	sub rbx, 1

	jmp .while
.end:
	; break
	ret


; Input
; (+8) eax - number
; (+4) rcx - ptr to buffer
int_to_hex_string:
	; eax - number
	; rcx - pointer
	; edx - remainder
	; rbx - 

	mov edx, 0
	mov rbx, 0

	mov eax, [rsp+8]
	add eax, 1  ; MAGIC NUMBER. Somewhy if you use "mov [rcx+rbx], dl" instead of "mov [buffer+rbx], dl" you will get number exactly on 1 less. This "add" "fix" that.

	mov rcx, [rsp+4]

hex_to_string_loop:
	div dword [const16]

	push rax

	; Convert int (digit) to ASCII char via hex_digits string
	mov dl, [hex_digits+rdx]


	; Move char to buffer
	mov byte [rcx+rbx], dl
	add rbx, 1

	pop rax
	mov rdx, 0

	; if remainder - EDX == 0: quit, else: recursive
	test eax, eax
	je .hex_to_string_exit
	; else
	jmp hex_to_string_loop
.hex_to_string_exit:
	; then

;	mov word [rcx+rbx], "x0"

	push rcx
	call reverse_string
	add rsp, 4

	ret


; Input
; (+8) eax - number
; (+4) rcx - ptr to buffer
int_to_bin_string:

	mov edx, 0
	mov rbx, 0

	mov eax, [rsp+8]
	add eax, 1  ; MAGIC NUMBER. Somewhy if you use "mov [rcx+rbx], dl" instead of "mov [buffer+rbx], dl" you will get number exactly on 1 less. This "add" "fix" that.

	mov rcx, [rsp+4]

int_to_bin_string_loop:
	div dword [const2]

	push rax

	; Convert int (digit) to ASCII char
	add edx, '0'

	; Move char to buffer
	mov [rcx+rbx], dl
	add rbx, 1

	pop rax
	mov rdx, 0

	; if remainder - EDX == 0: quit, else: recursive
	test eax, eax
	je .int_to_bin_string_exit
	; else
	jmp int_to_bin_string_loop
.int_to_bin_string_exit:
	; then
	push rcx
	call reverse_string
	add rsp, 4

	ret