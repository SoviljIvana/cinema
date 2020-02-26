import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { Container, Row, Col, Card, Button } from 'react-bootstrap';

class ProjectionDetails extends Component {

    constructor(props) {
        super(props);
        this.state = {
            movies: []
        };
    }

    getProjections() {
        const projectionTimes = ['11:45', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30', '12:25', '14:52', '17:30'];
        return projectionTimes.map((time) => {
            return <Button className="mr-1 mb-2">{time}</Button>
        })
    }

    render() {
        const projectionTimes = this.getProjections();
        return (
            <Container>
                <Row className="justify-content-center">
                    <Col>
                        <Card className="mt-5 card-width">
                            <Card.Body>
                                <Card.Text>
                                    <Row className="mt-2">
                                        <Col className="justify-content-center align-content-center">
                                            <div>
                                                <Row>
                                                    {projectionTimes}                     
                                               </Row>
                                            </div>
                                        </Col>
                                    </Row>
                                    <hr />
                                </Card.Text>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Container>
        );
    }
}

export default ProjectionDetails;
