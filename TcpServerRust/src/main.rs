use std::io::{Error, Read, Write};
use std::net::{TcpListener, TcpStream};
use std::thread;

fn main() {
    let tcp_server = TcpListener::bind("0.0.0.0:33333").expect("Failed to create tcp server.");
    tcp_server.incoming().into_iter().for_each(|stream| {
        match stream {
            Err(e) => {eprintln!("Error occured. Error: {}", e )}
            Ok(stream) => {
                thread::spawn(move || {
                    handler(stream).unwrap_or_else(|error| eprintln!("{:?}", error))
                });   
            }
        }
    });
}

fn handler(mut stream: TcpStream) -> Result<(), Error> {
    println!("Connection from {}", stream.peer_addr()?);
    let mut buffer = [0; 1024];
        loop {
        let nbytes = stream.read(&mut buffer)?;
        if nbytes == 0 {
            return Ok(());
        }
        stream.write(&buffer[..nbytes])?;
        stream.flush()?;
    }
}