import struct, zlib
p="/workspace/WaterPipe-Android/app/src/main/res/drawable-nodpi/allwater.png"
data=open(p,"rb").read(); pos=8; idat=b""; w=h=None
while pos<len(data):
    ln=struct.unpack(">I",data[pos:pos+4])[0]; typ=data[pos+4:pos+8]; chunk=data[pos+8:pos+8+ln]
    if typ==b"IHDR": w,h,bitdepth,ct,cf,fl,inter=struct.unpack(">IIBBBBB",chunk)
    elif typ==b"IDAT": idat+=chunk
    elif typ==b"IEND": break
    pos+=12+ln
raw=zlib.decompress(idat); bpp=4; stride=w*bpp
def paeth(a,b,c):
    pa,pb,pc=b-a,c-b,abs(a+b-2*c)
    if pa<pb:return a if pa<pc else c
    return b if pb<pc else c
out=bytearray(len(raw)); prev=bytearray(stride); rl=stride+1
for y in range(h):
    row=raw[y*rl:(y+1)*rl]; ft=row[0]; line=bytearray(row[1:])
    for x in range(stride):
        a=line[x-bpp] if x>=bpp else 0; b=prev[x]; c=prev[x-bpp] if x>=bpp else 0
        if ft==1:line[x]=(line[x]+a)&0xFF
        elif ft==2:line[x]=(line[x]+b)&0xFF
        elif ft==3:line[x]=(line[x]+((a+b)>>1))&0xFF
        elif ft==4:line[x]=(line[x]+paeth(a,b,c))&0xFF
    out[y*stride:(y+1)*stride]=line; prev=line

img=out
def write_png(path, x0,y0,x1,y1, scale=4):
    W=x1-x0; H=y1-y0
    # build RGBA rows with black checkerboard bg so alpha is visible
    rows=bytearray()
    for yy in range(H):
        rows.append(0)  # filter none
        for xx in range(W):
            i=((y0+yy)*stride)+(x0+xx)*bpp
            r=img[i];g=img[i+1];b=img[i+2];al=img[i+3]
            # composite over checkered bg
            cbi=0x33 if ((xx//8 + yy//8)%2)==0 else 0x88
            r=(r*al + cbi*(255-al))//255
            g=(g*al + cbi*(255-al))//255
            b=(b*al + cbi*(255-al))//255
            rows+=bytes((r,g,b,255))
    def chunk(t,data):
        c=struct.pack(">I",len(data))+t+data
        return c+struct.pack(">I",zlib.crc32(t+data)&0xffffffff)
    ihdr=struct.pack(">IIBBBBB",W,H,8,6,0,0,0)
    png=bytearray(b"\x89PNG\r\n\x1a\n")
    png+=chunk(b"IHDR",ihdr)
    png+=chunk(b"IDAT",zlib.compress(bytes(rows)))
    png+=chunk(b"IEND",b"")
    open(path,"wb").write(png)
    print("wrote",path,W,"x",H)

write_png("/workspace/crop_row0_ltile.png", 0,0,75,75)      # row0 col0
write_png("/workspace/crop_row0_col14.png", 1050,0,1125,75) # row0 col14
write_png("/workspace/crop_row3_col0.png", 0,225,75,300)     # row3 col0
write_png("/workspace/crop_row3_col14.png", 1050,225,1125,300) # row3 col14
write_png("/workspace/crop_row4.png", 0,300,1125,375)        # full row4 (straight lines)
write_png("/workspace/crop_row0.png", 0,0,1125,75)           # full row0