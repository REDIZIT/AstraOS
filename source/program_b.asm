section .data
const10: dd 10

section .text
global program_b_main

program_b_main:
	mov rbx, "z"
	call printCharacter
	ret

;Input
; rax = number to display

printNumber:
	push rbx
	push rdx
	xor rdx,rdx          ;edx:eax = number
	div dword [const10]  ;eax = quotient, edx = remainder
	test rbx,rbx         ;Is quotient zero?
	je .l1               ; yes, don't display it
	call printNumber     ;Display the quotient
.l1:
	lea rbx,[rdx+'0']
	call printCharacter  ;Display the remainder
	pop rdx
	pop rax


printCharacter:
	mov rdi, 0xB8000
	mov [rdi], rbx
	mov byte [rdi+1], 0x07
	ret