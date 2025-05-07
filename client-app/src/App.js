import React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Card, Button } from 'react-bootstrap';

function App() {
    return (
        <div className="container mt-5">
            <h1 className="mb-4">Featured Properties</h1>
            <div className="row">
                {[1, 2, 3, 4, 5, 6].map((id) => (
                    <div key={id} className="col-md-4 mb-4">
                        <Card>
                            <Card.Img variant="top" src={`https://source.unsplash.com/random?house,${id}`} />
                            <Card.Body>
                                <Card.Title>Property {id}</Card.Title>
                                <Card.Text>
                                    Beautiful and affordable home located at a prime location.
                                </Card.Text>
                                <Button variant="primary">View Details</Button>
                            </Card.Body>
                        </Card>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default App;
