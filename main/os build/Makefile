LDFLAGS = -nostdlib -n -T tools/link.ld
STRIPFLAGS = -s -K mmio -K fb -K bootboot -K environment -K initstack

all: myos.elf

myos.elf:
	rm -f tools/tmp/project.o
	rm -f tools/boot/myos.elf
	rm -f build/myos.img
	rm -f build/myos.vdi

	nasm -f elf64 src/nasm/project.nasm -o tools/tmp/project.o
	ld -r -b binary -o tools/tmp/font.o src/font.psf
	ld $(LDFLAGS) tools/tmp/font.o tools/tmp/project.o -o tools/boot/myos.elf 

	tools/mkbootimg tools/mkbootimg_cfg.json build/myos.img
	VBoxManage convertfromraw --format VDI build/myos.img build/myos.vdi --uuid=2a5b2242-ead3-4f96-9663-159554846c48

	qemu-system-x86_64 build/myos.img